using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Externos;
using SocialCare.Application.Interfaces;

namespace SocialCare.Infrastructure.ExternalApis;

public class BrasilApiClient : IBrasilApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<BrasilApiClient> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public BrasilApiClient(HttpClient http, ILogger<BrasilApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    // BrasilAPI tem rate limit baixo no endpoint de CNPJ (429). Tentamos algumas vezes
    // com backoff curto antes de desistir, para o usuário não precisar reenviar manualmente.
    private const int MaxTentativas = 4;
    private static readonly TimeSpan EsperaMaxima = TimeSpan.FromSeconds(8);

    public async Task<CnpjResponse?> ConsultarCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var digitos = DocumentoFiscal.SomenteDigitos(cnpj);
        if (digitos.Length != 14) return null;

        for (var tentativa = 1; ; tentativa++)
        {
            try
            {
                using var resp = await _http.GetAsync($"cnpj/v1/{digitos}", ct);

                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("CNPJ {Cnpj} não encontrado na BrasilAPI (404).", digitos);
                    return null;
                }

                if (resp.StatusCode == HttpStatusCode.TooManyRequests && tentativa < MaxTentativas)
                {
                    var espera = CalcularEspera(resp, tentativa);
                    if (espera <= EsperaMaxima)
                    {
                        _logger.LogWarning(
                            "BrasilAPI 429 ao consultar CNPJ {Cnpj}; tentativa {Tentativa}/{Max}, aguardando {Espera}s.",
                            digitos, tentativa, MaxTentativas, espera.TotalSeconds);
                        await Task.Delay(espera, ct);
                        continue;
                    }
                }

                resp.EnsureSuccessStatusCode();

                var p = await resp.Content.ReadFromJsonAsync<CnpjPayload>(JsonOpts, ct);
                if (p is null) return null;

                var endereco = MontarEndereco(p);
                return new CnpjResponse(
                    DocumentoFiscal.FormatarCnpj(digitos),
                    p.RazaoSocial,
                    p.NomeFantasia,
                    p.SituacaoCadastral,
                    p.Logradouro,
                    p.Numero,
                    p.Bairro,
                    p.Municipio,
                    p.Uf,
                    p.Cep,
                    p.Telefone,
                    p.Email,
                    endereco);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // Esgotaram as tentativas: o limite é por IP e não é culpa do CNPJ.
                _logger.LogWarning(ex, "Limite de consultas da BrasilAPI atingido ao consultar CNPJ {Cnpj}", digitos);
                throw new ExternalServiceException(
                    "Limite de consultas à BrasilAPI atingido. Aguarde cerca de 1 minuto e tente novamente.");
            }
            catch (Exception ex) when (ex is not ExternalServiceException)
            {
                // Qualquer outra falha (rede, timeout, parse, 5xx) não é "não encontrado":
                // loga a causa real e propaga em vez de mascarar como CNPJ inexistente.
                _logger.LogError(ex, "Falha ao consultar CNPJ {Cnpj} na BrasilAPI", digitos);
                throw new ExternalServiceException(
                    "Não foi possível consultar o CNPJ na BrasilAPI no momento. Tente novamente.", ex);
            }
        }
    }

    // Usa o header Retry-After quando presente; senão aplica backoff exponencial (2s, 4s, 8s...).
    private static TimeSpan CalcularEspera(HttpResponseMessage resp, int tentativa)
    {
        var retryAfter = resp.Headers.RetryAfter;
        if (retryAfter?.Delta is { } delta && delta > TimeSpan.Zero)
            return delta;
        if (retryAfter?.Date is { } date)
        {
            var ate = date - DateTimeOffset.UtcNow;
            if (ate > TimeSpan.Zero) return ate;
        }
        return TimeSpan.FromSeconds(Math.Pow(2, tentativa));
    }

    public async Task<IReadOnlyList<FeriadoDto>> ListarFeriadosAsync(int ano, CancellationToken ct = default)
    {
        try
        {
            var feriados = await _http.GetFromJsonAsync<List<FeriadoPayload>>($"feriados/v1/{ano}", JsonOpts, ct);
            return (feriados ?? [])
                .Where(f => DateTime.TryParse(f.Date, out _))
                .Select(f => new FeriadoDto(DateTime.Parse(f.Date!), f.Name ?? string.Empty, f.Type ?? string.Empty))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar feriados de {Ano} na BrasilAPI", ano);
            return [];
        }
    }

    private static string MontarEndereco(CnpjPayload p)
    {
        var partes = new[]
        {
            string.Join(", ", new[] { p.Logradouro, p.Numero }.Where(s => !string.IsNullOrWhiteSpace(s))),
            p.Bairro,
            string.Join("/", new[] { p.Municipio, p.Uf }.Where(s => !string.IsNullOrWhiteSpace(s)))
        };
        return string.Join(" - ", partes.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private class CnpjPayload
    {
        [JsonPropertyName("razao_social")] public string? RazaoSocial { get; set; }
        [JsonPropertyName("nome_fantasia")] public string? NomeFantasia { get; set; }
        [JsonPropertyName("descricao_situacao_cadastral")] public string? SituacaoCadastral { get; set; }
        [JsonPropertyName("logradouro")] public string? Logradouro { get; set; }
        [JsonPropertyName("numero")] public string? Numero { get; set; }
        [JsonPropertyName("bairro")] public string? Bairro { get; set; }
        [JsonPropertyName("municipio")] public string? Municipio { get; set; }
        [JsonPropertyName("uf")] public string? Uf { get; set; }
        [JsonPropertyName("cep")] public string? Cep { get; set; }
        [JsonPropertyName("ddd_telefone_1")] public string? Telefone { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
    }

    private class FeriadoPayload
    {
        [JsonPropertyName("date")] public string? Date { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
    }
}

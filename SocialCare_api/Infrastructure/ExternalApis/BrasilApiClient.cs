using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SocialCare.Application.Common;
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

    public async Task<CnpjResponse?> ConsultarCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var digitos = DocumentoFiscal.SomenteDigitos(cnpj);
        if (digitos.Length != 14) return null;

        try
        {
            var p = await _http.GetFromJsonAsync<CnpjPayload>($"cnpj/v1/{digitos}", JsonOpts, ct);
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
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar CNPJ {Cnpj} na BrasilAPI", digitos);
            return null;
        }
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

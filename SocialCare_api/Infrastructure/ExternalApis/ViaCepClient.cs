using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Application.Interfaces;

namespace SocialCare.Infrastructure.ExternalApis;

public class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ViaCepClient> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public ViaCepClient(HttpClient http, ILogger<ViaCepClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<CepResponse?> ConsultarAsync(string cep, CancellationToken ct = default)
    {
        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        if (cepLimpo.Length != 8) return null;

        try
        {
            var payload = await _http.GetFromJsonAsync<ViaCepPayload>($"{cepLimpo}/json/", JsonOpts, ct);
            if (payload is null || payload.Erro) return null;

            return new CepResponse(
                Cep: payload.Cep ?? cepLimpo,
                Logradouro: payload.Logradouro ?? string.Empty,
                Complemento: payload.Complemento ?? string.Empty,
                Bairro: payload.Bairro ?? string.Empty,
                Localidade: payload.Localidade ?? string.Empty,
                Uf: payload.Uf ?? string.Empty,
                Ibge: payload.Ibge,
                Estado: payload.Estado);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar CEP {Cep}", cepLimpo);
            return null;
        }
    }

    private class ViaCepPayload
    {
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; }
        public string? Uf { get; set; }
        public string? Ibge { get; set; }
        public string? Estado { get; set; }

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}

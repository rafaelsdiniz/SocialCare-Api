using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SocialCare.Application.Interfaces;

namespace SocialCare.Infrastructure.ExternalApis;

/// <summary>
/// Cliente do Portal da Transparência do Governo Federal (api.portaldatransparencia.gov.br).
/// Requer chave gratuita em <c>PortalTransparencia:ChaveApi</c>. Sem chave, fica desabilitado.
/// </summary>
public class PortalTransparenciaClient : IPortalTransparenciaClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PortalTransparenciaClient> _logger;
    private readonly string? _chave;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public PortalTransparenciaClient(HttpClient http, IConfiguration config, ILogger<PortalTransparenciaClient> logger)
    {
        _http = http;
        _logger = logger;
        _chave = config["PortalTransparencia:ChaveApi"];
    }

    public bool Habilitado => !string.IsNullOrWhiteSpace(_chave);

    public async Task<BolsaFamiliaMunicipio?> ObterBolsaFamiliaAsync(int codigoIbge, CancellationToken ct = default)
    {
        if (!Habilitado) return null;

        // Os dados têm defasagem de alguns meses — tenta do mês anterior para trás até achar.
        var referencia = DateTime.UtcNow.AddMonths(-1);
        for (var tentativa = 0; tentativa < 8; tentativa++, referencia = referencia.AddMonths(-1))
        {
            var mesAno = $"{referencia:yyyyMM}";
            try
            {
                var url = $"api-de-dados/bolsa-familia-por-municipio?mesAno={mesAno}&codigoIbge={codigoIbge}&pagina=1";
                var itens = await _http.GetFromJsonAsync<List<BolsaFamiliaPayload>>(url, JsonOpts, ct);
                var item = itens?.FirstOrDefault();
                if (item is { QuantidadeBeneficiados: > 0 })
                    return new BolsaFamiliaMunicipio(codigoIbge, item.QuantidadeBeneficiados, item.Valor, referencia.Year, referencia.Month);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao consultar Bolsa Família ({MesAno}, {Codigo}) no Portal da Transparência", mesAno, codigoIbge);
                return null;
            }
        }

        return null;
    }

    private class BolsaFamiliaPayload
    {
        [JsonPropertyName("quantidadeBeneficiados")] public int QuantidadeBeneficiados { get; set; }
        [JsonPropertyName("valor")] public decimal Valor { get; set; }
    }
}

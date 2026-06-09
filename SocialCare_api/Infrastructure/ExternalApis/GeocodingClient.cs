using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using SocialCare.Application.Interfaces;

namespace SocialCare.Infrastructure.ExternalApis;

/// <summary>
/// Geocodificação via Nominatim (OpenStreetMap) — gratuito, sem chave.
/// A política de uso exige User-Agent identificável (configurado no DI) e no máx. 1 req/s.
/// </summary>
public class GeocodingClient : IGeocodingClient
{
    private readonly HttpClient _http;
    private readonly ILogger<GeocodingClient> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public GeocodingClient(HttpClient http, ILogger<GeocodingClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<Coordenada?> GeocodificarAsync(GeocodingRequest e, CancellationToken ct = default)
    {
        // Busca estruturada melhora a precisão e evita ambiguidade.
        var rua = string.Join(" ", new[] { e.Numero, e.Logradouro }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        var q = HttpUtility.ParseQueryString(string.Empty);
        q["format"] = "jsonv2";
        q["limit"] = "1";
        q["countrycodes"] = "br";
        q["addressdetails"] = "0";
        if (!string.IsNullOrWhiteSpace(rua)) q["street"] = rua;
        if (!string.IsNullOrWhiteSpace(e.Municipio)) q["city"] = e.Municipio;
        if (!string.IsNullOrWhiteSpace(e.Uf)) q["state"] = e.Uf;
        var cep = new string((e.Cep ?? string.Empty).Where(char.IsDigit).ToArray());
        if (cep.Length == 8) q["postalcode"] = cep;

        try
        {
            var resultados = await _http.GetFromJsonAsync<List<NominatimItem>>($"search?{q}", JsonOpts, ct);
            var item = resultados?.FirstOrDefault();
            if (item is null) return await TentarPorMunicipioAsync(e, ct);

            if (TentarCoordenada(item, out var coord)) return coord;
            return await TentarPorMunicipioAsync(e, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao geocodificar endereço em {Municipio}/{Uf}", e.Municipio, e.Uf);
            return null;
        }
    }

    /// <summary>Fallback: ao menos posiciona no município quando o logradouro não é encontrado.</summary>
    private async Task<Coordenada?> TentarPorMunicipioAsync(GeocodingRequest e, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(e.Municipio)) return null;
        try
        {
            var q = HttpUtility.ParseQueryString(string.Empty);
            q["format"] = "jsonv2";
            q["limit"] = "1";
            q["countrycodes"] = "br";
            q["city"] = e.Municipio;
            q["state"] = e.Uf;
            var resultados = await _http.GetFromJsonAsync<List<NominatimItem>>($"search?{q}", JsonOpts, ct);
            return resultados?.FirstOrDefault() is { } item && TentarCoordenada(item, out var coord) ? coord : null;
        }
        catch
        {
            return null;
        }
    }

    private static bool TentarCoordenada(NominatimItem item, out Coordenada coord)
    {
        coord = new Coordenada(0, 0);
        if (double.TryParse(item.Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
            double.TryParse(item.Lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
        {
            coord = new Coordenada(lat, lon);
            return true;
        }
        return false;
    }

    private class NominatimItem
    {
        [JsonPropertyName("lat")] public string? Lat { get; set; }
        [JsonPropertyName("lon")] public string? Lon { get; set; }
    }
}

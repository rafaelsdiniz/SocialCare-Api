namespace SocialCare.Application.Interfaces;

public interface IGeocodingClient
{
    /// <summary>Geocodifica um endereço (Nominatim/OpenStreetMap). Retorna null se não localizar.</summary>
    Task<Coordenada?> GeocodificarAsync(GeocodingRequest endereco, CancellationToken ct = default);
}

public record GeocodingRequest(
    string Logradouro,
    string? Numero,
    string Bairro,
    string Municipio,
    string Uf,
    string? Cep);

public record Coordenada(double Latitude, double Longitude);

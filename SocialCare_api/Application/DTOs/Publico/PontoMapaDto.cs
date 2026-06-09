namespace SocialCare.Application.DTOs.Publico;

/// <summary>
/// Ponto agregado e anonimizado para o mapa público: coordenada arredondada (~100 m) e
/// quantidade de famílias ativas naquela vizinhança. Não identifica domicílios.
/// </summary>
public record PontoMapaDto(double Lat, double Lng, string Municipio, int Familias);

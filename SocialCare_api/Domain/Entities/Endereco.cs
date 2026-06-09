namespace SocialCare.Domain.Entities;

public class Endereco : BaseEntity
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string? PontoReferencia { get; set; }

    /// <summary>Coordenadas geográficas (geocodificadas via Nominatim/OSM). Nulas quando não localizadas.</summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int MunicipioId { get; set; }
    public Municipio Municipio { get; set; } = null!;

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;
}

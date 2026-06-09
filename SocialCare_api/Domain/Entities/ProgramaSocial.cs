namespace SocialCare.Domain.Entities;

public class ProgramaSocial : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Requisitos { get; set; }
    public string OrgaoResponsavel { get; set; } = string.Empty;
    /// <summary>Ícone do programa em data URL base64 (ex.: "data:image/png;base64,...").</summary>
    public string? IconeBase64 { get; set; }
    public decimal? ValorPadrao { get; set; }
    public int? DuracaoMesesPadrao { get; set; }
    public DateTime? VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }

    public ICollection<Beneficio> Beneficios { get; set; } = new List<Beneficio>();
}

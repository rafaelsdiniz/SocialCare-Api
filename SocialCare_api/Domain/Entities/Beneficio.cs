using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Beneficio : BaseEntity
{
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal Valor { get; set; }
    public StatusBeneficio Status { get; set; } = StatusBeneficio.EmAnalise;
    public string? Observacao { get; set; }
    public string? MotivoEncerramento { get; set; }

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int ProgramaSocialId { get; set; }
    public ProgramaSocial ProgramaSocial { get; set; } = null!;

    public int? AprovadoPorUsuarioId { get; set; }
    public Usuario? AprovadoPor { get; set; }
}

using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Encaminhamento : BaseEntity
{
    public DateTime DataEncaminhamento { get; set; } = DateTime.UtcNow;
    public string Motivo { get; set; } = string.Empty;
    public string? Demanda { get; set; }
    public StatusEncaminhamento Status { get; set; } = StatusEncaminhamento.Enviado;
    public DateTime? DataRetorno { get; set; }
    public string? Retorno { get; set; }

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int InstituicaoParceiraId { get; set; }
    public InstituicaoParceira InstituicaoParceira { get; set; } = null!;

    public int? MembroId { get; set; }
    public Membro? Membro { get; set; }
}

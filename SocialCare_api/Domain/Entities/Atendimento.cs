using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Atendimento : BaseEntity
{
    public DateTime DataAtendimento { get; set; } = DateTime.UtcNow;
    public string Motivo { get; set; } = string.Empty;
    public string? Parecer { get; set; }
    public string? Demanda { get; set; }
    public bool Remoto { get; set; }
    public StatusAtendimento Status { get; set; } = StatusAtendimento.Aberto;

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int AssistenteResponsavelId { get; set; }
    public Usuario AssistenteResponsavel { get; set; } = null!;
}

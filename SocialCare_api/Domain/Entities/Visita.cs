using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Visita : BaseEntity
{
    public DateTime DataAgendada { get; set; }
    public DateTime? DataRealizacao { get; set; }
    public TipoVisita Tipo { get; set; } = TipoVisita.Domiciliar;
    public StatusVisita Status { get; set; } = StatusVisita.Agendada;
    public string? Motivo { get; set; }
    public string? Observacoes { get; set; }
    public string? Encaminhamentos { get; set; }

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int AssistenteResponsavelId { get; set; }
    public Usuario AssistenteResponsavel { get; set; } = null!;
}

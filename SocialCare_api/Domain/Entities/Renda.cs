namespace SocialCare.Domain.Entities;

public class Renda : BaseEntity
{
    public decimal Valor { get; set; }
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public string? Fonte { get; set; }
    public string? Observacao { get; set; }

    public int MembroId { get; set; }
    public Membro Membro { get; set; } = null!;

    public int TipoRendaId { get; set; }
    public TipoRenda TipoRenda { get; set; } = null!;
}

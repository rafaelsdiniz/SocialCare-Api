namespace SocialCare.Domain.Entities;

public class FamiliaVulnerabilidade
{
    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int VulnerabilidadeId { get; set; }
    public Vulnerabilidade Vulnerabilidade { get; set; } = null!;

    public DateTime IdentificadaEm { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }
    public bool Resolvida { get; set; }
    public DateTime? ResolvidaEm { get; set; }
}

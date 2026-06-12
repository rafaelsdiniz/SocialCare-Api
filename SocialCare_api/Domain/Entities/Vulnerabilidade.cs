namespace SocialCare.Domain.Entities;

public class Vulnerabilidade : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Severidade { get; set; } = 1;

    public ICollection<FamiliaVulnerabilidade> Familias { get; set; } = new List<FamiliaVulnerabilidade>();
}

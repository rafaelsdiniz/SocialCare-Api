namespace SocialCare.Domain.Entities;

public class TipoRenda : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool ConsideradaParaCalculo { get; set; } = true;

    public ICollection<Renda> Rendas { get; set; } = new List<Renda>();
}

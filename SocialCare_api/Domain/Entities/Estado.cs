namespace SocialCare.Domain.Entities;

public class Estado : BaseEntity
{
    public int CodigoIbge { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;

    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}

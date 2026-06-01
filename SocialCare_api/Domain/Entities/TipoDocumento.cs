namespace SocialCare.Domain.Entities;

public class TipoDocumento : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string? MascaraValidacao { get; set; }

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}

namespace SocialCare.Domain.Entities;

public class Documento : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public string? OrgaoEmissor { get; set; }
    public DateTime? DataEmissao { get; set; }
    public DateTime? DataValidade { get; set; }

    public int MembroId { get; set; }
    public Membro Membro { get; set; } = null!;

    public int TipoDocumentoId { get; set; }
    public TipoDocumento TipoDocumento { get; set; } = null!;
}

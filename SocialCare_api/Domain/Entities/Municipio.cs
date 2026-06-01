namespace SocialCare.Domain.Entities;

public class Municipio : BaseEntity
{
    public int CodigoIbge { get; set; }
    public string Nome { get; set; } = string.Empty;

    public int EstadoId { get; set; }
    public Estado Estado { get; set; } = null!;

    public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
}

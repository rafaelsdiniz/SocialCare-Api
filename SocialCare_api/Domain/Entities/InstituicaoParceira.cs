namespace SocialCare.Domain.Entities;

public class InstituicaoParceira : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string AreaAtuacao { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? ResponsavelContato { get; set; }
    public string? EnderecoCompleto { get; set; }

    public ICollection<Encaminhamento> Encaminhamentos { get; set; } = new List<Encaminhamento>();
}

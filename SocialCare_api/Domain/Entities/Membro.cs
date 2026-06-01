using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Membro : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public Sexo Sexo { get; set; } = Sexo.NaoInformado;
    public EstadoCivil EstadoCivil { get; set; } = EstadoCivil.Solteiro;
    public string? NomeMae { get; set; }
    public string? NomePai { get; set; }
    public string? Escolaridade { get; set; }
    public string? Ocupacao { get; set; }
    public bool PessoaComDeficiencia { get; set; }
    public string? DescricaoDeficiencia { get; set; }
    public string? Telefone { get; set; }

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int ParentescoId { get; set; }
    public Parentesco Parentesco { get; set; } = null!;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    public ICollection<Renda> Rendas { get; set; } = new List<Renda>();
}

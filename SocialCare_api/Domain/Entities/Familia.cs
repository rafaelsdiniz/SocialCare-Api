using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class Familia : BaseEntity
{
    public string CodigoFamiliar { get; set; } = string.Empty;
    public string NomeResponsavel { get; set; } = string.Empty;
    public int QuantidadeMembros { get; set; }
    public decimal RendaTotalMensal { get; set; }
    public decimal RendaPerCapita { get; set; }
    public StatusFamilia Status { get; set; } = StatusFamilia.Ativa;
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int? MembroResponsavelId { get; set; }
    public Membro? MembroResponsavel { get; set; }

    public Endereco? Endereco { get; set; }
    public ICollection<Membro> Membros { get; set; } = new List<Membro>();
    public ICollection<FamiliaVulnerabilidade> Vulnerabilidades { get; set; } = new List<FamiliaVulnerabilidade>();
    public ICollection<Beneficio> Beneficios { get; set; } = new List<Beneficio>();
    public ICollection<Visita> Visitas { get; set; } = new List<Visita>();
    public ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
    public ICollection<Encaminhamento> Encaminhamentos { get; set; } = new List<Encaminhamento>();
}

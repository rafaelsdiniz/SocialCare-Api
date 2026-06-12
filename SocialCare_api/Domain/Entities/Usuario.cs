namespace SocialCare.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime? UltimoLoginEm { get; set; }

    public ICollection<UsuarioPerfil> UsuarioPerfis { get; set; } = new List<UsuarioPerfil>();
    public ICollection<LogAuditoria> Auditorias { get; set; } = new List<LogAuditoria>();
    public ICollection<Visita> VisitasResponsavel { get; set; } = new List<Visita>();
    public ICollection<Atendimento> AtendimentosResponsavel { get; set; } = new List<Atendimento>();
}

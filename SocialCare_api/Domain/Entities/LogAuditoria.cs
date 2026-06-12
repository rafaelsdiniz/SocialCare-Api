using SocialCare.Domain.Enums;

namespace SocialCare.Domain.Entities;

public class LogAuditoria : BaseEntity
{
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public TipoAuditoria Tipo { get; set; }
    public string Entidade { get; set; } = string.Empty;
    public string? EntidadeId { get; set; }
    public string? DadosAntes { get; set; }
    public string? DadosDepois { get; set; }
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
}

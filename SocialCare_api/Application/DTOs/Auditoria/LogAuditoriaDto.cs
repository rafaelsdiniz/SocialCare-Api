namespace SocialCare.Application.DTOs.Auditoria;

public record LogAuditoriaDto(
    int Id,
    int? UsuarioId,
    string? Usuario,
    string Tipo,
    string Entidade,
    string? EntidadeId,
    string? DadosDepois,
    string? EnderecoIp,
    string? UserAgent,
    DateTime CriadoEm);

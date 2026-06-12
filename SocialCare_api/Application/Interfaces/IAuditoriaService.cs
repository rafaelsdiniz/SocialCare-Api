using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Auditoria;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record AuditoriaFiltro(int? UsuarioId = null, string? Entidade = null, TipoAuditoria? Tipo = null);

public interface IAuditoriaService
{
    Task<PagedResult<LogAuditoriaDto>> ListarAsync(AuditoriaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
}

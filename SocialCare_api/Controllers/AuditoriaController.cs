using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Auditoria;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/auditoria")]
[Authorize(Roles = Perfis.Administrador)]
public class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaService _service;

    public AuditoriaController(IAuditoriaService service)
    {
        _service = service;
    }

    /// <summary>Lista a trilha de auditoria com filtro por usuário, entidade e tipo.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LogAuditoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<LogAuditoriaDto>>> Listar(
        [FromQuery] int? usuarioId,
        [FromQuery] string? entidade,
        [FromQuery] TipoAuditoria? tipo,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new AuditoriaFiltro(usuarioId, entidade, tipo), paginacao, ct));
}

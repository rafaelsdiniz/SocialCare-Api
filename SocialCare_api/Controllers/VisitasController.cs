using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Visitas;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/visitas")]
[Authorize(Roles = Perfis.Operacao)]
public class VisitasController : ControllerBase
{
    private readonly IVisitaService _service;

    public VisitasController(IVisitaService service)
    {
        _service = service;
    }

    /// <summary>Lista visitas com filtro por família, assistente e status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VisitaResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<VisitaResumoDto>>> Listar(
        [FromQuery] int? familiaId,
        [FromQuery] int? assistenteId,
        [FromQuery] StatusVisita? status,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new VisitaFiltro(familiaId, assistenteId, status), paginacao, ct));

    /// <summary>Obtém uma visita (inclui aviso de feriado, se aplicável).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VisitaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitaResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Agenda uma visita domiciliar. O assistente responsável é o usuário autenticado.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VisitaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<VisitaResponse>> Agendar([FromBody] AgendarVisitaRequest request, CancellationToken ct)
    {
        var criada = await _service.AgendarAsync(request, User.ObterUsuarioId(), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Edita uma visita agendada.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VisitaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VisitaResponse>> Atualizar(int id, [FromBody] AtualizarVisitaRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Registra a realização de uma visita.</summary>
    [HttpPost("{id:int}/registrar")]
    [ProducesResponseType(typeof(VisitaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VisitaResponse>> Registrar(int id, [FromBody] RegistrarVisitaRequest request, CancellationToken ct)
        => Ok(await _service.RegistrarAsync(id, request, ct));

    /// <summary>Cancela uma visita.</summary>
    [HttpPost("{id:int}/cancelar")]
    [ProducesResponseType(typeof(VisitaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VisitaResponse>> Cancelar(int id, CancellationToken ct)
        => Ok(await _service.CancelarAsync(id, ct));
}

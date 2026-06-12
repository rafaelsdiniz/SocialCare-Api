using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Programas;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/programas")]
[Authorize(Roles = Perfis.Gestao)]
public class ProgramasController : ControllerBase
{
    private readonly IProgramaService _service;

    public ProgramasController(IProgramaService service)
    {
        _service = service;
    }

    /// <summary>Lista programas sociais com filtro e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProgramaResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProgramaResumoDto>>> Listar(
        [FromQuery] string? busca,
        [FromQuery] bool? ativo,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new ProgramaFiltro(busca, ativo), paginacao, ct));

    /// <summary>Obtém um programa social.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProgramaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgramaResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra um novo programa social.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProgramaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProgramaResponse>> Criar([FromBody] CriarProgramaRequest request, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um programa social.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProgramaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProgramaResponse>> Atualizar(int id, [FromBody] AtualizarProgramaRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Inativa um programa social.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}

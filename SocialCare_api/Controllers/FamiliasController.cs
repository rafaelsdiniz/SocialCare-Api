using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Familias;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/familias")]
[Authorize(Roles = Perfis.Operacao)]
public class FamiliasController : ControllerBase
{
    private readonly IFamiliaService _service;

    public FamiliasController(IFamiliaService service)
    {
        _service = service;
    }

    /// <summary>Lista famílias com filtro por status/busca e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FamiliaResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FamiliaResumoDto>>> Listar(
        [FromQuery] string? busca,
        [FromQuery] StatusFamilia? status,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
    {
        var resultado = await _service.ListarAsync(new FamiliaFiltro(busca, status), paginacao, ct);
        return Ok(resultado);
    }

    /// <summary>Obtém uma família pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FamiliaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FamiliaResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra uma nova família com endereço.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FamiliaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FamiliaResponse>> Criar([FromBody] CriarFamiliaRequest request, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza os dados de uma família.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FamiliaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FamiliaResponse>> Atualizar(int id, [FromBody] AtualizarFamiliaRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Inativa (exclusão lógica) uma família.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Gestao)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}

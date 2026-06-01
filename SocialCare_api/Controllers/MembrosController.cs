using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Membros;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/familias/{familiaId:int}/membros")]
[Authorize(Roles = Perfis.Operacao)]
public class MembrosController : ControllerBase
{
    private readonly IMembroService _service;

    public MembrosController(IMembroService service)
    {
        _service = service;
    }

    /// <summary>Lista os membros ativos de uma família.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MembroResumoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MembroResumoDto>>> Listar(int familiaId, CancellationToken ct)
        => Ok(await _service.ListarPorFamiliaAsync(familiaId, ct));

    /// <summary>Obtém um membro da família.</summary>
    [HttpGet("{membroId:int}")]
    [ProducesResponseType(typeof(MembroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembroResponse>> Obter(int familiaId, int membroId, CancellationToken ct)
        => Ok(await _service.ObterAsync(familiaId, membroId, ct));

    /// <summary>Adiciona um membro (com documentos e rendas) e recalcula a renda da família.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MembroResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MembroResponse>> Criar(int familiaId, [FromBody] CriarMembroRequest request, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(familiaId, request, ct);
        return CreatedAtAction(nameof(Obter), new { familiaId, membroId = criado.Id }, criado);
    }

    /// <summary>Atualiza um membro e recalcula a renda da família.</summary>
    [HttpPut("{membroId:int}")]
    [ProducesResponseType(typeof(MembroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembroResponse>> Atualizar(int familiaId, int membroId, [FromBody] AtualizarMembroRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(familiaId, membroId, request, ct));

    /// <summary>Remove (exclusão lógica) um membro e recalcula a renda da família.</summary>
    [HttpDelete("{membroId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int familiaId, int membroId, CancellationToken ct)
    {
        await _service.RemoverAsync(familiaId, membroId, ct);
        return NoContent();
    }
}

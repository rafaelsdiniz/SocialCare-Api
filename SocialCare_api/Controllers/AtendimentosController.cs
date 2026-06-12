using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Atendimentos;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/atendimentos")]
[Authorize(Roles = Perfis.Operacao)]
public class AtendimentosController : ControllerBase
{
    private readonly IAtendimentoService _service;

    public AtendimentosController(IAtendimentoService service)
    {
        _service = service;
    }

    /// <summary>Lista atendimentos com filtro por família, assistente e status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AtendimentoResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AtendimentoResumoDto>>> Listar(
        [FromQuery] int? familiaId,
        [FromQuery] int? assistenteId,
        [FromQuery] StatusAtendimento? status,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new AtendimentoFiltro(familiaId, assistenteId, status), paginacao, ct));

    /// <summary>Obtém um atendimento.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AtendimentoResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Abre um atendimento. O assistente responsável é o usuário autenticado.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AtendimentoResponse>> Abrir([FromBody] AbrirAtendimentoRequest request, CancellationToken ct)
    {
        var criado = await _service.AbrirAsync(request, User.ObterUsuarioId(), ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um atendimento (motivo, parecer, status). Concluir exige parecer.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AtendimentoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AtendimentoResponse>> Atualizar(int id, [FromBody] AtualizarAtendimentoRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));
}

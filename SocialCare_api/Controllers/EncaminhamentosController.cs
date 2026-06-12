using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Encaminhamentos;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/encaminhamentos")]
[Authorize(Roles = Perfis.Operacao)]
public class EncaminhamentosController : ControllerBase
{
    private readonly IEncaminhamentoService _service;

    public EncaminhamentosController(IEncaminhamentoService service)
    {
        _service = service;
    }

    /// <summary>Lista encaminhamentos com filtro por família, instituição e status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EncaminhamentoResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EncaminhamentoResumoDto>>> Listar(
        [FromQuery] int? familiaId,
        [FromQuery] int? instituicaoParceiraId,
        [FromQuery] StatusEncaminhamento? status,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new EncaminhamentoFiltro(familiaId, instituicaoParceiraId, status), paginacao, ct));

    /// <summary>Obtém um encaminhamento.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EncaminhamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EncaminhamentoResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Registra um encaminhamento de família/membro para uma instituição parceira.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EncaminhamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EncaminhamentoResponse>> Criar([FromBody] CriarEncaminhamentoRequest request, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Registra o retorno/atualização de status de um encaminhamento.</summary>
    [HttpPut("{id:int}/retorno")]
    [ProducesResponseType(typeof(EncaminhamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EncaminhamentoResponse>> RegistrarRetorno(int id, [FromBody] RegistrarRetornoRequest request, CancellationToken ct)
        => Ok(await _service.RegistrarRetornoAsync(id, request, ct));
}

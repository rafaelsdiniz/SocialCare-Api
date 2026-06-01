using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Beneficios;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/beneficios")]
[Authorize(Roles = Perfis.Gestao)]
public class BeneficiosController : ControllerBase
{
    private readonly IBeneficioService _service;

    public BeneficiosController(IBeneficioService service)
    {
        _service = service;
    }

    /// <summary>Lista benefícios com filtro por família, programa e status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BeneficioResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BeneficioResumoDto>>> Listar(
        [FromQuery] int? familiaId,
        [FromQuery] int? programaSocialId,
        [FromQuery] StatusBeneficio? status,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new BeneficioFiltro(familiaId, programaSocialId, status), paginacao, ct));

    /// <summary>Obtém um benefício.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeneficioResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Concede (registra em análise) um benefício a uma família.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BeneficioResponse>> Conceder([FromBody] ConcederBeneficioRequest request, CancellationToken ct)
    {
        var criado = await _service.ConcederAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Edita um benefício (apenas enquanto em análise).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BeneficioResponse>> Atualizar(int id, [FromBody] AtualizarBeneficioRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Aprova um benefício em análise (registra o aprovador).</summary>
    [HttpPost("{id:int}/aprovar")]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BeneficioResponse>> Aprovar(int id, CancellationToken ct)
        => Ok(await _service.AprovarAsync(id, User.ObterUsuarioId(), ct));

    /// <summary>Indefere um benefício em análise.</summary>
    [HttpPost("{id:int}/indeferir")]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BeneficioResponse>> Indeferir(int id, [FromBody] AlterarStatusBeneficioRequest request, CancellationToken ct)
        => Ok(await _service.IndeferirAsync(id, request.Motivo, ct));

    /// <summary>Encerra um benefício ativo/aprovado/suspenso.</summary>
    [HttpPost("{id:int}/encerrar")]
    [ProducesResponseType(typeof(BeneficioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BeneficioResponse>> Encerrar(int id, [FromBody] AlterarStatusBeneficioRequest request, CancellationToken ct)
        => Ok(await _service.EncerrarAsync(id, request.Motivo, ct));
}

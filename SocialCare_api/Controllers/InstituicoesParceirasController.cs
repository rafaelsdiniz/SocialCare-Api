using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Externos;
using SocialCare.Application.DTOs.Instituicoes;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/instituicoes-parceiras")]
[Authorize(Roles = Perfis.Gestao)]
public class InstituicoesParceirasController : ControllerBase
{
    private readonly IInstituicaoService _service;

    public InstituicoesParceirasController(IInstituicaoService service)
    {
        _service = service;
    }

    /// <summary>Lista instituições parceiras com filtro e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InstituicaoResumoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<InstituicaoResumoDto>>> Listar(
        [FromQuery] string? busca,
        [FromQuery] bool? ativo,
        [FromQuery] PaginacaoQuery paginacao,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(new InstituicaoFiltro(busca, ativo), paginacao, ct));

    /// <summary>Consulta dados de um CNPJ na BrasilAPI para pré-preenchimento do cadastro.</summary>
    [HttpGet("consulta-cnpj/{cnpj}")]
    [ProducesResponseType(typeof(CnpjResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CnpjResponse>> ConsultarCnpj(string cnpj, CancellationToken ct)
        => Ok(await _service.ConsultarCnpjAsync(cnpj, ct));

    /// <summary>Obtém uma instituição parceira.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InstituicaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstituicaoResponse>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra uma instituição parceira.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(InstituicaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InstituicaoResponse>> Criar([FromBody] CriarInstituicaoRequest request, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza uma instituição parceira.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(InstituicaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstituicaoResponse>> Atualizar(int id, [FromBody] AtualizarInstituicaoRequest request, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, request, ct));

    /// <summary>Inativa uma instituição parceira.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}

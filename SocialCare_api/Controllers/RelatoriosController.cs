using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Relatorios;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Roles = Perfis.Gestao)]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _service;

    public RelatoriosController(IRelatorioService service)
    {
        _service = service;
    }

    /// <summary>Quantidade de famílias (ativas) por vulnerabilidade.</summary>
    [HttpGet("familias-por-vulnerabilidade")]
    [ProducesResponseType(typeof(IEnumerable<FamiliasPorVulnerabilidadeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FamiliasPorVulnerabilidadeDto>>> FamiliasPorVulnerabilidade(CancellationToken ct)
        => Ok(await _service.FamiliasPorVulnerabilidadeAsync(ct));

    /// <summary>Quantidade e valor de benefícios por programa social.</summary>
    [HttpGet("beneficios-por-programa")]
    [ProducesResponseType(typeof(IEnumerable<BeneficiosPorProgramaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BeneficiosPorProgramaDto>>> BeneficiosPorPrograma(CancellationToken ct)
        => Ok(await _service.BeneficiosPorProgramaAsync(ct));

    /// <summary>Quantidade de visitas por assistente, segmentada por status.</summary>
    [HttpGet("visitas-por-assistente")]
    [ProducesResponseType(typeof(IEnumerable<VisitasPorAssistenteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VisitasPorAssistenteDto>>> VisitasPorAssistente(CancellationToken ct)
        => Ok(await _service.VisitasPorAssistenteAsync(ct));
}

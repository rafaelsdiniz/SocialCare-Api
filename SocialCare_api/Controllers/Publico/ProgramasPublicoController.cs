using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/programas")]
[AllowAnonymous]
public class ProgramasPublicoController : ControllerBase
{
    private const string CacheKey = "publico:programas";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public ProgramasPublicoController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>Lista programas sociais ativos disponíveis (vitrine pública). Cache 1h.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProgramaPublicoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProgramaPublicoDto>>> Listar(CancellationToken ct)
    {
        var hoje = DateTime.UtcNow.Date;
        var programas = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _db.ProgramasSociais
                .AsNoTracking()
                .Where(p => p.Ativo &&
                    (p.VigenciaInicio == null || p.VigenciaInicio <= hoje) &&
                    (p.VigenciaFim == null || p.VigenciaFim >= hoje))
                .OrderBy(p => p.Nome)
                .Select(p => new ProgramaPublicoDto(
                    p.Id, p.Nome, p.Descricao, p.OrgaoResponsavel, p.Requisitos, p.ValorPadrao))
                .ToListAsync(ct);
        });

        return Ok(programas ?? new List<ProgramaPublicoDto>());
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/estados")]
[AllowAnonymous]
public class EstadosController : ControllerBase
{
    private const string CacheKey = "publico:estados";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public EstadosController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>Lista todas as UFs cadastradas (cache 24h).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstadoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstadoDto>>> Listar(CancellationToken ct)
    {
        var estados = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _db.Estados
                .AsNoTracking()
                .OrderBy(e => e.Nome)
                .Select(e => new EstadoDto(e.Id, e.CodigoIbge, e.Sigla, e.Nome, e.Regiao))
                .ToListAsync(ct);
        });

        return Ok(estados);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/municipios")]
[AllowAnonymous]
public class MunicipiosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public MunicipiosController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>Lista municípios da UF informada (cache 24h).</summary>
    [HttpGet("{ufSigla}")]
    [ProducesResponseType(typeof(IEnumerable<MunicipioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MunicipioDto>>> Listar(string ufSigla, CancellationToken ct)
    {
        ufSigla = (ufSigla ?? string.Empty).Trim().ToUpperInvariant();
        if (ufSigla.Length != 2) return BadRequest(new { erro = "UF inválida." });

        var cacheKey = $"publico:municipios:{ufSigla}";
        var municipios = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _db.Municipios
                .AsNoTracking()
                .Where(m => m.Estado.Sigla == ufSigla)
                .OrderBy(m => m.Nome)
                .Select(m => new MunicipioDto(m.Id, m.CodigoIbge, m.Nome, m.Estado.Sigla))
                .ToListAsync(ct);
        });

        if (municipios is null || municipios.Count == 0)
            return NotFound(new { erro = $"Nenhum município encontrado para UF '{ufSigla}'." });

        return Ok(municipios);
    }
}

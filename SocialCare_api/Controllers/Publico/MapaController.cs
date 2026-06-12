using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Domain.Enums;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/mapa")]
[AllowAnonymous]
public class MapaController : ControllerBase
{
    private const string CacheKey = "publico:mapa";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public MapaController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>
    /// Pontos agregados e anonimizados das famílias ativas geocodificadas (coordenada arredondada
    /// para ~100 m e agrupada). Alimenta o mapa público "onde atuamos". Cache 1h.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PontoMapaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PontoMapaDto>>> Listar(CancellationToken ct)
    {
        var pontos = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var brutos = await _db.Enderecos
                .Where(e => e.Familia.Status == StatusFamilia.Ativa && e.Latitude != null && e.Longitude != null)
                .Select(e => new { Lat = e.Latitude!.Value, Lng = e.Longitude!.Value, Municipio = e.Municipio.Nome })
                .ToListAsync(ct);

            // Arredonda para 3 casas (~110 m) e agrega — preserva a privacidade do domicílio.
            return brutos
                .GroupBy(p => new { Lat = Math.Round(p.Lat, 3), Lng = Math.Round(p.Lng, 3), p.Municipio })
                .Select(g => new PontoMapaDto(g.Key.Lat, g.Key.Lng, g.Key.Municipio, g.Count()))
                .ToList();
        });

        return Ok(pontos ?? new List<PontoMapaDto>());
    }
}

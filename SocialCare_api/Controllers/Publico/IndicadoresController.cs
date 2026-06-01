using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Domain.Enums;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/indicadores")]
[AllowAnonymous]
public class IndicadoresController : ControllerBase
{
    private const string CacheKey = "publico:indicadores";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    public IndicadoresController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>Estatísticas agregadas e anonimizadas para parceiros/BIs. Cache 1h.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IndicadoresDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<IndicadoresDto>> Obter(CancellationToken ct)
    {
        var resultado = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var totalFamilias = await _db.Familias.CountAsync(f => f.Status == StatusFamilia.Ativa, ct);
            var totalMembros = await _db.Membros.CountAsync(m => m.Ativo, ct);
            var totalBeneficios = await _db.Beneficios.CountAsync(b => b.Status == StatusBeneficio.Ativo, ct);
            var totalProgramas = await _db.ProgramasSociais.CountAsync(p => p.Ativo, ct);

            var inicioJanela = DateTime.UtcNow.AddDays(-30);
            var totalVisitas = await _db.Visitas
                .CountAsync(v => v.DataAgendada >= inicioJanela, ct);

            var siglasUf = await _db.Enderecos
                .Where(e => e.Familia.Status == StatusFamilia.Ativa)
                .Select(e => e.Municipio.Estado.Sigla)
                .ToListAsync(ct);
            var porUf = siglasUf
                .GroupBy(s => s)
                .Select(g => new IndicadorPorUfDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            var nomesProgramas = await _db.Beneficios
                .Where(b => b.Status == StatusBeneficio.Ativo)
                .Select(b => b.ProgramaSocial.Nome)
                .ToListAsync(ct);
            var porPrograma = nomesProgramas
                .GroupBy(n => n)
                .Select(g => new IndicadorPorProgramaDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            return new IndicadoresDto(
                totalFamilias, totalMembros, totalBeneficios, totalProgramas, totalVisitas,
                porUf, porPrograma);
        });

        return Ok(resultado);
    }
}

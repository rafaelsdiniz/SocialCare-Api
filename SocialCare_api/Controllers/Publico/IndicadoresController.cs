using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Enums;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/indicadores")]
[AllowAnonymous]
public class IndicadoresController : ControllerBase
{
    private const string CacheKey = "publico:indicadores";
    private const string CacheKeyContexto = "publico:contexto-federal";
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly IIbgeClient _ibge;
    private readonly IPortalTransparenciaClient _portal;

    public IndicadoresController(AppDbContext db, IMemoryCache cache, IIbgeClient ibge, IPortalTransparenciaClient portal)
    {
        _db = db;
        _cache = cache;
        _ibge = ibge;
        _portal = portal;
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

            var nomesMunicipios = await _db.Enderecos
                .Where(e => e.Familia.Status == StatusFamilia.Ativa)
                .Select(e => e.Municipio.Nome)
                .ToListAsync(ct);
            var porMunicipio = nomesMunicipios
                .GroupBy(n => n)
                .Select(g => new IndicadorPorMunicipioDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Quantidade)
                .Take(10)
                .ToList();

            var statusFamilias = await _db.Familias.Select(f => f.Status).ToListAsync(ct);
            var porStatus = statusFamilias
                .GroupBy(s => s)
                .Select(g => new IndicadorPorStatusDto(RotuloStatus(g.Key), g.Count()))
                .OrderByDescending(x => x.Quantidade)
                .ToList();

            var populacaoAbrangida = await CalcularPopulacaoAbrangidaAsync(ct);

            return new IndicadoresDto(
                totalFamilias, totalMembros, totalBeneficios, totalProgramas, totalVisitas, populacaoAbrangida,
                porUf, porPrograma, porMunicipio, porStatus);
        });

        return Ok(resultado);
    }

    /// <summary>Contexto federal (Portal da Transparência): Bolsa Família pago nos municípios atendidos. Cache 12h.</summary>
    [HttpGet("contexto-federal")]
    [ProducesResponseType(typeof(ContextoFederalDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContextoFederalDto>> ContextoFederal(CancellationToken ct)
    {
        if (!_portal.Habilitado)
            return Ok(new ContextoFederalDto(false, 0, 0, 0m, null, null));

        var resultado = await _cache.GetOrCreateAsync(CacheKeyContexto, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

            var codigosIbge = await _db.Enderecos
                .Where(e => e.Familia.Status == StatusFamilia.Ativa)
                .Select(e => e.Municipio.CodigoIbge)
                .Distinct()
                .ToListAsync(ct);

            var consultados = 0;
            var beneficiarios = 0;
            var valor = 0m;
            int? ano = null;
            int? mes = null;

            foreach (var codigo in codigosIbge)
            {
                var bf = await _portal.ObterBolsaFamiliaAsync(codigo, ct);
                if (bf is null) continue;
                consultados++;
                beneficiarios += bf.Beneficiarios;
                valor += bf.Valor;
                // Usa o mês de referência mais recente encontrado entre os municípios.
                if (ano is null || bf.Ano > ano || (bf.Ano == ano && bf.Mes > mes))
                {
                    ano = bf.Ano;
                    mes = bf.Mes;
                }
            }

            return new ContextoFederalDto(consultados > 0, consultados, beneficiarios, valor, ano, mes);
        });

        return Ok(resultado);
    }

    /// <summary>Soma da população (IBGE) dos municípios com famílias ativas — base para a taxa de cobertura.</summary>
    private async Task<int> CalcularPopulacaoAbrangidaAsync(CancellationToken ct)
    {
        var codigosIbge = await _db.Enderecos
            .Where(e => e.Familia.Status == StatusFamilia.Ativa)
            .Select(e => e.Municipio.CodigoIbge)
            .Distinct()
            .ToListAsync(ct);

        var total = 0;
        foreach (var codigo in codigosIbge)
        {
            // População muda muito devagar — cache longo por município evita repetir a chamada ao IBGE.
            var pop = await _cache.GetOrCreateAsync($"ibge:pop:{codigo}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                return await _ibge.ObterPopulacaoAsync(codigo, ct);
            });
            total += pop ?? 0;
        }
        return total;
    }

    private static string RotuloStatus(StatusFamilia status) => status switch
    {
        StatusFamilia.Ativa => "Ativa",
        StatusFamilia.EmAcompanhamento => "Em acompanhamento",
        StatusFamilia.Inativa => "Inativa",
        StatusFamilia.Desligada => "Desligada",
        _ => status.ToString(),
    };
}

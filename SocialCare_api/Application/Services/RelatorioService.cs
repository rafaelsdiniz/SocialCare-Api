using Microsoft.EntityFrameworkCore;
using SocialCare.Application.DTOs.Relatorios;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IRepository<Vulnerabilidade> _vulnerabilidades;
    private readonly IRepository<ProgramaSocial> _programas;
    private readonly IRepository<Usuario> _usuarios;

    public RelatorioService(
        IRepository<Vulnerabilidade> vulnerabilidades,
        IRepository<ProgramaSocial> programas,
        IRepository<Usuario> usuarios)
    {
        _vulnerabilidades = vulnerabilidades;
        _programas = programas;
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<FamiliasPorVulnerabilidadeDto>> FamiliasPorVulnerabilidadeAsync(CancellationToken ct = default)
    {
        // Projeção com subconsultas é materializada e ordenada em memória (conjunto de catálogo, pequeno).
        var dados = await _vulnerabilidades.Query()
            .Select(v => new FamiliasPorVulnerabilidadeDto(
                v.Id,
                v.Nome,
                v.Severidade,
                v.Familias.Count(fv => fv.Familia.Ativo)))
            .ToListAsync(ct);

        return dados
            .OrderByDescending(x => x.QuantidadeFamilias)
            .ThenByDescending(x => x.Severidade)
            .ToList();
    }

    public async Task<IReadOnlyList<BeneficiosPorProgramaDto>> BeneficiosPorProgramaAsync(CancellationToken ct = default)
    {
        var dados = await _programas.Query()
            .Select(p => new BeneficiosPorProgramaDto(
                p.Id,
                p.Nome,
                p.Beneficios.Count(),
                p.Beneficios.Count(b => b.Status == StatusBeneficio.Ativo),
                p.Beneficios.Where(b => b.Status == StatusBeneficio.Ativo).Sum(b => (decimal?)b.Valor) ?? 0m))
            .ToListAsync(ct);

        return dados.OrderByDescending(x => x.QuantidadeTotal).ToList();
    }

    public async Task<IReadOnlyList<VisitasPorAssistenteDto>> VisitasPorAssistenteAsync(CancellationToken ct = default)
    {
        var dados = await _usuarios.Query()
            .Where(u => u.VisitasResponsavel.Any())
            .Select(u => new VisitasPorAssistenteDto(
                u.Id,
                u.Nome,
                u.VisitasResponsavel.Count(),
                u.VisitasResponsavel.Count(v => v.Status == StatusVisita.Realizada),
                u.VisitasResponsavel.Count(v => v.Status == StatusVisita.Agendada),
                u.VisitasResponsavel.Count(v => v.Status == StatusVisita.Cancelada)))
            .ToListAsync(ct);

        return dados.OrderByDescending(x => x.Total).ToList();
    }
}

using SocialCare.Application.DTOs.Relatorios;

namespace SocialCare.Application.Interfaces;

public interface IRelatorioService
{
    Task<IReadOnlyList<FamiliasPorVulnerabilidadeDto>> FamiliasPorVulnerabilidadeAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BeneficiosPorProgramaDto>> BeneficiosPorProgramaAsync(CancellationToken ct = default);
    Task<IReadOnlyList<VisitasPorAssistenteDto>> VisitasPorAssistenteAsync(CancellationToken ct = default);
}

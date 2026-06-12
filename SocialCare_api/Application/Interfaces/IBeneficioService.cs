using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Beneficios;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record BeneficioFiltro(int? FamiliaId = null, int? ProgramaSocialId = null, StatusBeneficio? Status = null);

public interface IBeneficioService
{
    Task<PagedResult<BeneficioResumoDto>> ListarAsync(BeneficioFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<BeneficioResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<BeneficioResponse> ConcederAsync(ConcederBeneficioRequest request, CancellationToken ct = default);
    Task<BeneficioResponse> AtualizarAsync(int id, AtualizarBeneficioRequest request, CancellationToken ct = default);
    Task<BeneficioResponse> AprovarAsync(int id, int? aprovadoPorUsuarioId, CancellationToken ct = default);
    Task<BeneficioResponse> IndeferirAsync(int id, string? motivo, CancellationToken ct = default);
    Task<BeneficioResponse> EncerrarAsync(int id, string? motivo, CancellationToken ct = default);
}

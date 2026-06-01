using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Visitas;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record VisitaFiltro(int? FamiliaId = null, int? AssistenteId = null, StatusVisita? Status = null);

public interface IVisitaService
{
    Task<PagedResult<VisitaResumoDto>> ListarAsync(VisitaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<VisitaResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<VisitaResponse> AgendarAsync(AgendarVisitaRequest request, int? assistenteId, CancellationToken ct = default);
    Task<VisitaResponse> AtualizarAsync(int id, AtualizarVisitaRequest request, CancellationToken ct = default);
    Task<VisitaResponse> RegistrarAsync(int id, RegistrarVisitaRequest request, CancellationToken ct = default);
    Task<VisitaResponse> CancelarAsync(int id, CancellationToken ct = default);
}

using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Familias;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record FamiliaFiltro(string? Busca = null, StatusFamilia? Status = null);

public interface IFamiliaService
{
    Task<PagedResult<FamiliaResumoDto>> ListarAsync(FamiliaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<FamiliaResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<FamiliaResponse> CriarAsync(CriarFamiliaRequest request, CancellationToken ct = default);
    Task<FamiliaResponse> AtualizarAsync(int id, AtualizarFamiliaRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

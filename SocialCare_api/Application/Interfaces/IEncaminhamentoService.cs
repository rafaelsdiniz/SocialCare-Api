using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Encaminhamentos;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record EncaminhamentoFiltro(int? FamiliaId = null, int? InstituicaoParceiraId = null, StatusEncaminhamento? Status = null);

public interface IEncaminhamentoService
{
    Task<PagedResult<EncaminhamentoResumoDto>> ListarAsync(EncaminhamentoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<EncaminhamentoResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<EncaminhamentoResponse> CriarAsync(CriarEncaminhamentoRequest request, CancellationToken ct = default);
    Task<EncaminhamentoResponse> RegistrarRetornoAsync(int id, RegistrarRetornoRequest request, CancellationToken ct = default);
}

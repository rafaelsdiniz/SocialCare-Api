using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Atendimentos;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Interfaces;

public record AtendimentoFiltro(int? FamiliaId = null, int? AssistenteId = null, StatusAtendimento? Status = null);

public interface IAtendimentoService
{
    Task<PagedResult<AtendimentoResumoDto>> ListarAsync(AtendimentoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<AtendimentoResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<AtendimentoResponse> AbrirAsync(AbrirAtendimentoRequest request, int? assistenteId, CancellationToken ct = default);
    Task<AtendimentoResponse> AtualizarAsync(int id, AtualizarAtendimentoRequest request, CancellationToken ct = default);
}

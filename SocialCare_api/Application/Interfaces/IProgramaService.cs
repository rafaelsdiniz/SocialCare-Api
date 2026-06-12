using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Programas;

namespace SocialCare.Application.Interfaces;

public record ProgramaFiltro(string? Busca = null, bool? Ativo = null);

public interface IProgramaService
{
    Task<PagedResult<ProgramaResumoDto>> ListarAsync(ProgramaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<ProgramaResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<ProgramaResponse> CriarAsync(CriarProgramaRequest request, CancellationToken ct = default);
    Task<ProgramaResponse> AtualizarAsync(int id, AtualizarProgramaRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Externos;
using SocialCare.Application.DTOs.Instituicoes;

namespace SocialCare.Application.Interfaces;

public record InstituicaoFiltro(string? Busca = null, bool? Ativo = null);

public interface IInstituicaoService
{
    Task<PagedResult<InstituicaoResumoDto>> ListarAsync(InstituicaoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<InstituicaoResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<InstituicaoResponse> CriarAsync(CriarInstituicaoRequest request, CancellationToken ct = default);
    Task<InstituicaoResponse> AtualizarAsync(int id, AtualizarInstituicaoRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);

    /// <summary>Consulta dados de um CNPJ na BrasilAPI para pré-preenchimento (não persiste).</summary>
    Task<CnpjResponse> ConsultarCnpjAsync(string cnpj, CancellationToken ct = default);
}

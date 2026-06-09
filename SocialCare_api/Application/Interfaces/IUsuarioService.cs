using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Usuarios;

namespace SocialCare.Application.Interfaces;

public record UsuarioFiltro(string? Busca = null, bool? Ativo = null);

public interface IUsuarioService
{
    Task<PagedResult<UsuarioResumoDto>> ListarAsync(UsuarioFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default);
    Task<UsuarioResponse> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> AtualizarAsync(int id, AtualizarUsuarioRequest request, CancellationToken ct = default);
    Task AlterarSenhaAsync(int id, AlterarSenhaRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> AtualizarMinhaContaAsync(int id, AtualizarMinhaContaRequest request, CancellationToken ct = default);
    Task AlterarMinhaSenhaAsync(int id, AlterarMinhaSenhaRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

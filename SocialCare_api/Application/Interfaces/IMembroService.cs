using SocialCare.Application.DTOs.Membros;

namespace SocialCare.Application.Interfaces;

public interface IMembroService
{
    Task<IReadOnlyList<MembroResumoDto>> ListarPorFamiliaAsync(int familiaId, CancellationToken ct = default);
    Task<MembroResponse> ObterAsync(int familiaId, int membroId, CancellationToken ct = default);
    Task<MembroResponse> CriarAsync(int familiaId, CriarMembroRequest request, CancellationToken ct = default);
    Task<MembroResponse> AtualizarAsync(int familiaId, int membroId, AtualizarMembroRequest request, CancellationToken ct = default);
    Task RemoverAsync(int familiaId, int membroId, CancellationToken ct = default);
}

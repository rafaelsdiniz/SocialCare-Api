using SocialCare.Application.DTOs.Externos;

namespace SocialCare.Application.Interfaces;

public interface IBrasilApiClient
{
    /// <summary>Consulta dados cadastrais de um CNPJ. Retorna null se não encontrado ou em caso de falha.</summary>
    Task<CnpjResponse?> ConsultarCnpjAsync(string cnpj, CancellationToken ct = default);

    /// <summary>Lista os feriados nacionais de um ano. Retorna lista vazia em caso de falha.</summary>
    Task<IReadOnlyList<FeriadoDto>> ListarFeriadosAsync(int ano, CancellationToken ct = default);
}

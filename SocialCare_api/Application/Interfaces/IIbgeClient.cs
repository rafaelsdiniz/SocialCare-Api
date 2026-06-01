namespace SocialCare.Application.Interfaces;

public interface IIbgeClient
{
    Task<IReadOnlyList<IbgeEstado>> ListarEstadosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<IbgeMunicipio>> ListarMunicipiosAsync(string ufSigla, CancellationToken ct = default);
}

public record IbgeEstado(int Id, string Sigla, string Nome, string Regiao);
public record IbgeMunicipio(int Id, string Nome);

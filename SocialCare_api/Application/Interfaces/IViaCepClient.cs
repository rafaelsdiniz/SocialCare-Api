using SocialCare.Application.DTOs.Publico;

namespace SocialCare.Application.Interfaces;

public interface IViaCepClient
{
    Task<CepResponse?> ConsultarAsync(string cep, CancellationToken ct = default);
}

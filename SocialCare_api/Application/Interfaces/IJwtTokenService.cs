using SocialCare.Domain.Entities;

namespace SocialCare.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiraEm) Gerar(Usuario usuario, IEnumerable<string> perfis);
}

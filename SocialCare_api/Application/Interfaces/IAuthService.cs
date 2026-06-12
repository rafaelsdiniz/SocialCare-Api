using SocialCare.Application.DTOs.Auth;

namespace SocialCare.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> AutenticarAsync(LoginRequest request, CancellationToken ct = default);
}

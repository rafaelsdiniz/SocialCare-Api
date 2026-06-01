using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.DTOs.Auth;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Entities;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AuthService(AppDbContext db, IJwtTokenService jwt, IPasswordHasher<Usuario> hasher)
    {
        _db = db;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<LoginResponse?> AutenticarAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.UsuarioPerfis).ThenInclude(up => up.Perfil)
            .FirstOrDefaultAsync(u => u.Login == request.Login && u.Ativo, ct);

        if (usuario is null) return null;

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha);
        if (resultado == PasswordVerificationResult.Failed) return null;

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.SenhaHash = _hasher.HashPassword(usuario, request.Senha);
        }

        usuario.UltimoLoginEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var perfis = usuario.UsuarioPerfis.Select(up => up.Perfil.Nome).ToList();
        var (token, expira) = _jwt.Gerar(usuario, perfis);

        return new LoginResponse(
            token,
            expira,
            new UsuarioAutenticadoDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, perfis));
    }
}

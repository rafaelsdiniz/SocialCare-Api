using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialCare.Domain.Entities;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Infrastructure.Bootstrap;

public static class AdminBootstrap
{
    public static async Task GarantirAdminAsync(IServiceProvider sp, IConfiguration config, ILogger logger, CancellationToken ct = default)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        var login = config["Admin:Login"] ?? "admin";
        var email = config["Admin:Email"] ?? "admin@socialcare.local";
        var senha = config["Admin:SenhaInicial"] ?? "ChangeMe@123";
        var nome = config["Admin:Nome"] ?? "Administrador";

        var existe = await db.Usuarios.AnyAsync(u => u.Login == login, ct);
        if (existe)
        {
            logger.LogInformation("Admin '{Login}' já existe. Nada a fazer.", login);
            return;
        }

        var perfilAdmin = await db.Perfis.FirstOrDefaultAsync(p => p.Nome == "Administrador", ct);
        if (perfilAdmin is null)
        {
            logger.LogWarning("Perfil 'Administrador' não encontrado. Execute as migrations antes do bootstrap.");
            return;
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Login = login,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };
        usuario.SenhaHash = hasher.HashPassword(usuario, senha);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);

        db.UsuarioPerfis.Add(new UsuarioPerfil
        {
            UsuarioId = usuario.Id,
            PerfilId = perfilAdmin.Id,
            AtribuidoEm = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Admin '{Login}' criado com senha inicial. Altere imediatamente em produção.", login);
    }
}

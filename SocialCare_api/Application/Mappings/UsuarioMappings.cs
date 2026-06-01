using SocialCare.Application.DTOs.Usuarios;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class UsuarioMappings
{
    private static List<string> NomesPerfis(Usuario u)
        => u.UsuarioPerfis
            .Where(up => up.Perfil is not null)
            .Select(up => up.Perfil.Nome)
            .ToList();

    public static UsuarioResumoDto ToResumoDto(this Usuario u)
        => new(u.Id, u.Nome, u.Login, u.Email, u.Ativo, NomesPerfis(u));

    public static UsuarioResponse ToResponse(this Usuario u)
        => new(
            u.Id,
            u.Nome,
            u.Login,
            u.Email,
            u.Ativo,
            u.UltimoLoginEm,
            NomesPerfis(u),
            u.CriadoEm,
            u.AtualizadoEm);
}

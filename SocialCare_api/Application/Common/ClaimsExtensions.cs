using System.Security.Claims;

namespace SocialCare.Application.Common;

public static class ClaimsExtensions
{
    /// <summary>Obtém o id do usuário autenticado a partir das claims do JWT.</summary>
    public static int? ObterUsuarioId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");
        return int.TryParse(valor, out var id) ? id : null;
    }
}

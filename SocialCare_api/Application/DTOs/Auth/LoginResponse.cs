namespace SocialCare.Application.DTOs.Auth;

public record LoginResponse(
    string Token,
    DateTime ExpiraEm,
    UsuarioAutenticadoDto Usuario);

public record UsuarioAutenticadoDto(
    int Id,
    string Nome,
    string Email,
    string Login,
    IReadOnlyList<string> Perfis);

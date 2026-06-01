namespace SocialCare.Application.DTOs.Usuarios;

public record CriarUsuarioRequest(
    string Nome,
    string Email,
    string Login,
    string Senha,
    IReadOnlyList<int> PerfilIds);

public record AtualizarUsuarioRequest(
    string Nome,
    string Email,
    IReadOnlyList<int> PerfilIds,
    bool Ativo);

public record AlterarSenhaRequest(string NovaSenha);

public record UsuarioResumoDto(
    int Id,
    string Nome,
    string Login,
    string Email,
    bool Ativo,
    IReadOnlyList<string> Perfis);

public record UsuarioResponse(
    int Id,
    string Nome,
    string Login,
    string Email,
    bool Ativo,
    DateTime? UltimoLoginEm,
    IReadOnlyList<string> Perfis,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

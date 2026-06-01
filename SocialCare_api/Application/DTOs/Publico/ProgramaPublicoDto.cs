namespace SocialCare.Application.DTOs.Publico;

public record ProgramaPublicoDto(
    int Id,
    string Nome,
    string? Descricao,
    string OrgaoResponsavel,
    string? Requisitos,
    decimal? ValorPadrao);

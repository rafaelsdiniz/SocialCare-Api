namespace SocialCare.Application.DTOs.Publico;

public record ProgramaPublicoDto(
    int Id,
    string Nome,
    string? Descricao,
    string OrgaoResponsavel,
    string? Requisitos,
    string? IconeBase64,
    decimal? ValorPadrao);

public record ProgramaPublicoDetalheDto(
    int Id,
    string Nome,
    string? Descricao,
    string OrgaoResponsavel,
    string? Requisitos,
    string? IconeBase64,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim);

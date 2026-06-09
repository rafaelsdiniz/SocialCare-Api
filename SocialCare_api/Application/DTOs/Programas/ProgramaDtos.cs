namespace SocialCare.Application.DTOs.Programas;

public record CriarProgramaRequest(
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    string? IconeBase64,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim);

public record AtualizarProgramaRequest(
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    string? IconeBase64,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim,
    bool Ativo);

public record ProgramaResumoDto(
    int Id,
    string Nome,
    string OrgaoResponsavel,
    string? IconeBase64,
    decimal? ValorPadrao,
    bool Ativo);

public record ProgramaResponse(
    int Id,
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    string? IconeBase64,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

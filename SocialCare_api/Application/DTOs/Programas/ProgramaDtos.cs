namespace SocialCare.Application.DTOs.Programas;

public record CriarProgramaRequest(
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim);

public record AtualizarProgramaRequest(
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim,
    bool Ativo);

public record ProgramaResumoDto(
    int Id,
    string Nome,
    string OrgaoResponsavel,
    decimal? ValorPadrao,
    bool Ativo);

public record ProgramaResponse(
    int Id,
    string Nome,
    string OrgaoResponsavel,
    string? Descricao,
    string? Requisitos,
    decimal? ValorPadrao,
    int? DuracaoMesesPadrao,
    DateTime? VigenciaInicio,
    DateTime? VigenciaFim,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

namespace SocialCare.Application.DTOs.Beneficios;

public record ConcederBeneficioRequest(
    int FamiliaId,
    int ProgramaSocialId,
    DateTime DataInicio,
    DateTime? DataFim,
    decimal Valor,
    string? Observacao);

public record AtualizarBeneficioRequest(
    DateTime DataInicio,
    DateTime? DataFim,
    decimal Valor,
    string? Observacao);

public record AlterarStatusBeneficioRequest(string? Motivo);

public record BeneficioResumoDto(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    int ProgramaSocialId,
    string? Programa,
    decimal Valor,
    string Status,
    DateTime DataInicio,
    DateTime? DataFim);

public record BeneficioResponse(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    string? FamiliaResponsavel,
    int ProgramaSocialId,
    string? Programa,
    decimal Valor,
    string Status,
    DateTime DataInicio,
    DateTime? DataFim,
    string? Observacao,
    string? MotivoEncerramento,
    int? AprovadoPorUsuarioId,
    string? AprovadoPor,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

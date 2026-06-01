using SocialCare.Domain.Enums;

namespace SocialCare.Application.DTOs.Visitas;

public record AgendarVisitaRequest(
    int FamiliaId,
    DateTime DataAgendada,
    TipoVisita Tipo,
    string? Motivo);

public record AtualizarVisitaRequest(
    DateTime DataAgendada,
    TipoVisita Tipo,
    string? Motivo,
    string? Observacoes);

public record RegistrarVisitaRequest(
    DateTime? DataRealizacao,
    string? Observacoes,
    string? Encaminhamentos);

public record VisitaResumoDto(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    string Tipo,
    string Status,
    DateTime DataAgendada,
    DateTime? DataRealizacao,
    string? Assistente);

public record VisitaResponse(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    int AssistenteResponsavelId,
    string? Assistente,
    string Tipo,
    string Status,
    DateTime DataAgendada,
    DateTime? DataRealizacao,
    string? Motivo,
    string? Observacoes,
    string? Encaminhamentos,
    string? Aviso,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

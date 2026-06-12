using SocialCare.Domain.Enums;

namespace SocialCare.Application.DTOs.Atendimentos;

public record AbrirAtendimentoRequest(
    int FamiliaId,
    DateTime? DataAtendimento,
    string Motivo,
    string? Demanda,
    bool Remoto);

public record AtualizarAtendimentoRequest(
    string Motivo,
    string? Demanda,
    string? Parecer,
    bool Remoto,
    StatusAtendimento Status);

public record AtendimentoResumoDto(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    string Motivo,
    string Status,
    bool Remoto,
    DateTime DataAtendimento,
    string? Assistente);

public record AtendimentoResponse(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    int AssistenteResponsavelId,
    string? Assistente,
    string Motivo,
    string? Demanda,
    string? Parecer,
    bool Remoto,
    string Status,
    DateTime DataAtendimento,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

using SocialCare.Domain.Enums;

namespace SocialCare.Application.DTOs.Encaminhamentos;

public record CriarEncaminhamentoRequest(
    int FamiliaId,
    int InstituicaoParceiraId,
    int? MembroId,
    string Motivo,
    string? Demanda,
    DateTime? DataEncaminhamento);

public record RegistrarRetornoRequest(
    StatusEncaminhamento Status,
    string? Retorno,
    DateTime? DataRetorno);

public record EncaminhamentoResumoDto(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    int InstituicaoParceiraId,
    string? Instituicao,
    string Motivo,
    string Status,
    DateTime DataEncaminhamento);

public record EncaminhamentoResponse(
    int Id,
    int FamiliaId,
    string? FamiliaCodigo,
    int InstituicaoParceiraId,
    string? Instituicao,
    int? MembroId,
    string? Membro,
    string Motivo,
    string? Demanda,
    string Status,
    DateTime DataEncaminhamento,
    DateTime? DataRetorno,
    string? Retorno,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

using SocialCare.Domain.Enums;

namespace SocialCare.Application.DTOs.Membros;

public record DocumentoRequest(
    int TipoDocumentoId,
    string Numero,
    string? OrgaoEmissor,
    DateTime? DataEmissao,
    DateTime? DataValidade);

public record RendaRequest(
    int TipoRendaId,
    decimal Valor,
    int MesReferencia,
    int AnoReferencia,
    string? Fonte,
    string? Observacao);

public record CriarMembroRequest(
    string Nome,
    DateTime DataNascimento,
    Sexo Sexo,
    EstadoCivil EstadoCivil,
    int ParentescoId,
    string? NomeMae,
    string? NomePai,
    string? Escolaridade,
    string? Ocupacao,
    bool PessoaComDeficiencia,
    string? DescricaoDeficiencia,
    string? Telefone,
    IReadOnlyList<DocumentoRequest>? Documentos,
    IReadOnlyList<RendaRequest>? Rendas);

public record AtualizarMembroRequest(
    string Nome,
    DateTime DataNascimento,
    Sexo Sexo,
    EstadoCivil EstadoCivil,
    int ParentescoId,
    string? NomeMae,
    string? NomePai,
    string? Escolaridade,
    string? Ocupacao,
    bool PessoaComDeficiencia,
    string? DescricaoDeficiencia,
    string? Telefone,
    IReadOnlyList<DocumentoRequest>? Documentos,
    IReadOnlyList<RendaRequest>? Rendas);

public record DocumentoDto(
    int Id,
    int TipoDocumentoId,
    string? TipoDocumento,
    string Numero,
    string? OrgaoEmissor,
    DateTime? DataEmissao,
    DateTime? DataValidade);

public record RendaDto(
    int Id,
    int TipoRendaId,
    string? TipoRenda,
    decimal Valor,
    int MesReferencia,
    int AnoReferencia,
    string? Fonte,
    string? Observacao,
    bool ConsideradaParaCalculo);

public record MembroResumoDto(
    int Id,
    string Nome,
    int Idade,
    string Sexo,
    string? Parentesco,
    decimal RendaMensalConsiderada);

public record MembroResponse(
    int Id,
    int FamiliaId,
    string Nome,
    DateTime DataNascimento,
    int Idade,
    string Sexo,
    string EstadoCivil,
    int ParentescoId,
    string? Parentesco,
    string? NomeMae,
    string? NomePai,
    string? Escolaridade,
    string? Ocupacao,
    bool PessoaComDeficiencia,
    string? DescricaoDeficiencia,
    string? Telefone,
    decimal RendaMensalConsiderada,
    IReadOnlyList<DocumentoDto> Documentos,
    IReadOnlyList<RendaDto> Rendas,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    bool Ativo);

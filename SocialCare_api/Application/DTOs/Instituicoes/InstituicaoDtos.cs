namespace SocialCare.Application.DTOs.Instituicoes;

public record CriarInstituicaoRequest(
    string Nome,
    string Cnpj,
    string AreaAtuacao,
    string? Telefone,
    string? Email,
    string? ResponsavelContato,
    string? EnderecoCompleto);

public record AtualizarInstituicaoRequest(
    string Nome,
    string Cnpj,
    string AreaAtuacao,
    string? Telefone,
    string? Email,
    string? ResponsavelContato,
    string? EnderecoCompleto,
    bool Ativo);

public record InstituicaoResumoDto(
    int Id,
    string Nome,
    string Cnpj,
    string AreaAtuacao,
    bool Ativo);

public record InstituicaoResponse(
    int Id,
    string Nome,
    string Cnpj,
    string AreaAtuacao,
    string? Telefone,
    string? Email,
    string? ResponsavelContato,
    string? EnderecoCompleto,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

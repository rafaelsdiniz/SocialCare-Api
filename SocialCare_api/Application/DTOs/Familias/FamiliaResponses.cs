namespace SocialCare.Application.DTOs.Familias;

/// <summary>Representação resumida da família (listagens).</summary>
public record FamiliaResumoDto(
    int Id,
    string CodigoFamiliar,
    string NomeResponsavel,
    int QuantidadeMembros,
    decimal RendaPerCapita,
    string Status,
    string? Municipio,
    string? Uf);

/// <summary>Representação completa da família.</summary>
public record FamiliaResponse(
    int Id,
    string CodigoFamiliar,
    string NomeResponsavel,
    int QuantidadeMembros,
    decimal RendaTotalMensal,
    decimal RendaPerCapita,
    string Status,
    string? Observacoes,
    DateTime DataCadastro,
    EnderecoDto? Endereco,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    bool Ativo);

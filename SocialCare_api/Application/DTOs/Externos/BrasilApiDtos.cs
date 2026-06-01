namespace SocialCare.Application.DTOs.Externos;

/// <summary>Dados cadastrais de um CNPJ retornados pela BrasilAPI (subconjunto útil).</summary>
public record CnpjResponse(
    string Cnpj,
    string? RazaoSocial,
    string? NomeFantasia,
    string? SituacaoCadastral,
    string? Logradouro,
    string? Numero,
    string? Bairro,
    string? Municipio,
    string? Uf,
    string? Cep,
    string? Telefone,
    string? Email,
    string? EnderecoCompleto);

/// <summary>Feriado nacional retornado pela BrasilAPI.</summary>
public record FeriadoDto(DateTime Data, string Nome, string Tipo);

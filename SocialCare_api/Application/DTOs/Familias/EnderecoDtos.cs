namespace SocialCare.Application.DTOs.Familias;

public record EnderecoRequest(
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string? PontoReferencia,
    int MunicipioId);

public record EnderecoDto(
    int Id,
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string? PontoReferencia,
    int MunicipioId,
    string? Municipio,
    string? Uf);

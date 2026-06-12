namespace SocialCare.Application.DTOs.Publico;

public record CepResponse(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Localidade,
    string Uf,
    string? Ibge,
    string? Estado);

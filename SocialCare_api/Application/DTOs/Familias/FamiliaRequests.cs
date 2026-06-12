using SocialCare.Domain.Enums;

namespace SocialCare.Application.DTOs.Familias;

public record CriarFamiliaRequest(
    string CodigoFamiliar,
    string NomeResponsavel,
    string? Observacoes,
    EnderecoRequest Endereco);

public record AtualizarFamiliaRequest(
    string NomeResponsavel,
    StatusFamilia Status,
    string? Observacoes,
    EnderecoRequest Endereco);

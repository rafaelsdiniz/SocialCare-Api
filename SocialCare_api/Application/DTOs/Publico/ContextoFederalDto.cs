namespace SocialCare.Application.DTOs.Publico;

/// <summary>
/// Contexto federal (Portal da Transparência): Bolsa Família pago nos municípios atendidos,
/// para dar dimensão pública aos números do sistema. <c>Disponivel=false</c> quando a integração
/// não está configurada.
/// </summary>
public record ContextoFederalDto(
    bool Disponivel,
    int MunicipiosConsultados,
    int Beneficiarios,
    decimal ValorTotal,
    int? Ano,
    int? Mes);

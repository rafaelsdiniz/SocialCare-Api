namespace SocialCare.Application.Interfaces;

public interface IPortalTransparenciaClient
{
    /// <summary>
    /// Bolsa Família pago em um município (código IBGE) no mês de referência mais recente disponível.
    /// Retorna null se não houver chave de API configurada ou se o Portal não responder.
    /// </summary>
    Task<BolsaFamiliaMunicipio?> ObterBolsaFamiliaAsync(int codigoIbge, CancellationToken ct = default);

    /// <summary>Indica se a integração está habilitada (chave-api-dados configurada).</summary>
    bool Habilitado { get; }
}

public record BolsaFamiliaMunicipio(int CodigoIbge, int Beneficiarios, decimal Valor, int Ano, int Mes);

using SocialCare.Application.Interfaces;
using SocialCare.Infrastructure.ExternalApis;

namespace SocialCare.Extensions;

public static class ExternalApisExtensions
{
    public static IServiceCollection AddSocialCareExternalApis(this IServiceCollection services, IConfiguration config)
    {
        var viaCepBaseUrl = config["ViaCep:BaseUrl"] ?? "https://viacep.com.br/ws/";
        var ibgeBaseUrl = config["Ibge:BaseUrl"] ?? "https://servicodados.ibge.gov.br/api/v1/";
        var brasilApiBaseUrl = config["BrasilApi:BaseUrl"] ?? "https://brasilapi.com.br/api/";
        var nominatimBaseUrl = config["Nominatim:BaseUrl"] ?? "https://nominatim.openstreetmap.org/";
        var nominatimUserAgent = config["Nominatim:UserAgent"] ?? "SocialCare/1.0 (assistencia-social)";
        var portalBaseUrl = config["PortalTransparencia:BaseUrl"] ?? "https://api.portaldatransparencia.gov.br/";
        var portalChave = config["PortalTransparencia:ChaveApi"];

        services.AddHttpClient<IViaCepClient, ViaCepClient>(c =>
        {
            c.BaseAddress = new Uri(viaCepBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddHttpClient<IIbgeClient, IbgeClient>(c =>
        {
            c.BaseAddress = new Uri(ibgeBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IBrasilApiClient, BrasilApiClient>(c =>
        {
            c.BaseAddress = new Uri(brasilApiBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(15);
        });

        // Geocodificação (Nominatim/OSM) — exige User-Agent identificável pela política de uso.
        services.AddHttpClient<IGeocodingClient, GeocodingClient>(c =>
        {
            c.BaseAddress = new Uri(nominatimBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(12);
            c.DefaultRequestHeaders.UserAgent.ParseAdd(nominatimUserAgent);
        });

        // Portal da Transparência — header chave-api-dados (quando configurado).
        services.AddHttpClient<IPortalTransparenciaClient, PortalTransparenciaClient>(c =>
        {
            c.BaseAddress = new Uri(portalBaseUrl);
            c.Timeout = TimeSpan.FromSeconds(15);
            if (!string.IsNullOrWhiteSpace(portalChave))
                c.DefaultRequestHeaders.Add("chave-api-dados", portalChave);
        });

        return services;
    }
}

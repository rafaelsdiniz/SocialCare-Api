using SocialCare.Application.Interfaces;
using SocialCare.Infrastructure.ExternalApis;

namespace SocialCare.Extensions;

public static class ExternalApisExtensions
{
    public static IServiceCollection AddSocialCareExternalApis(this IServiceCollection services, IConfiguration config)
    {
        var viaCepBaseUrl = config["ViaCep:BaseUrl"] ?? "https://viacep.com.br/ws/";
        var ibgeBaseUrl = config["Ibge:BaseUrl"] ?? "https://servicodados.ibge.gov.br/api/v1/";

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

        return services;
    }
}

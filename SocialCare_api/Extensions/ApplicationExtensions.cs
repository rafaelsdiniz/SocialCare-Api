using FluentValidation;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Services;

namespace SocialCare.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddSocialCareApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<FamiliaService>();

        services.AddScoped<IFamiliaService, FamiliaService>();

        return services;
    }
}

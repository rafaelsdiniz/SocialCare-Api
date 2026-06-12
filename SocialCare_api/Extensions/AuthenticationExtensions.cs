using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Services;
using SocialCare.Domain.Entities;
using SocialCare.Infrastructure.Identity;

namespace SocialCare.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddSocialCareAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(JwtSettings.Section);
        services.Configure<JwtSettings>(section);
        var settings = section.Get<JwtSettings>() ?? throw new InvalidOperationException("Seção 'Jwt' ausente em appsettings.");

        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();
        return services;
    }
}

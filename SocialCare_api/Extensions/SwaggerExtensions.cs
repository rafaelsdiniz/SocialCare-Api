using Microsoft.OpenApi;

namespace SocialCare.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSocialCareSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SocialCare API",
                Version = "v1",
                Description = "API de gestão de assistência social — Tópicos III."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Cole apenas o token JWT (sem o prefixo 'Bearer ')."
            });

            c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
            });
        });
        return services;
    }
}

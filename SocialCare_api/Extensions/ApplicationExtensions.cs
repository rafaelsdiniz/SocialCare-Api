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
        services.AddScoped<IMembroService, MembroService>();
        services.AddScoped<IProgramaService, ProgramaService>();
        services.AddScoped<IBeneficioService, BeneficioService>();
        services.AddScoped<IInstituicaoService, InstituicaoService>();
        services.AddScoped<IEncaminhamentoService, EncaminhamentoService>();
        services.AddScoped<IVisitaService, VisitaService>();
        services.AddScoped<IAtendimentoService, AtendimentoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        return services;
    }
}

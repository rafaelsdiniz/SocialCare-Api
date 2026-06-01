using SocialCare.Domain.Interfaces;
using SocialCare.Infrastructure.Repositories;

namespace SocialCare.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddSocialCarePersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}

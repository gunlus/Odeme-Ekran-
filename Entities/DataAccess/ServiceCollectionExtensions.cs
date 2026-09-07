using Microsoft.Extensions.DependencyInjection;
using Entities;

namespace DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        // DbContext ve tüm servisler DataAccess içinde kaydedilir
        services.AddDbContext<BankaDbContext>();
        services.AddScoped<OdemeServisi>();
        services.AddScoped<MusteriServisi>();
        services.AddScoped<HesapServisi>();
        services.AddScoped<LogServisi>();

        return services;
    }
}
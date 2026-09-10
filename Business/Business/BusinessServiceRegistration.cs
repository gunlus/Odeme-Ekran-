namespace Business;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DataAccess; 

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessAndDataAccessServices(this IServiceCollection services)
    {
        services.AddDbContext<BankaDbContext>(options =>
            options.UseSqlite("Data Source=banka.db")); 
            
        services.AddScoped<IHesapRepository, HesapRepository>();
        services.AddScoped<IMusteriRepository, MusteriRepository>();
        services.AddScoped<IOdemeRepository, OdemeRepository>();
        services.AddScoped<ILogRepository, LogRepository>();

        services.AddScoped<IHesapServisi, HesapServisi>();
        services.AddScoped<IMusteriServisi, MusteriServisi>();
        services.AddScoped<IOdemeServisi, OdemeServisi>();
        services.AddScoped<ILogServisi, LogServisi>();

        return services;
    }

    // YENİ: Veritabanını otomatik oluşturan metot
    public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BankaDbContext>();
        db.Database.EnsureCreated();
        
    }
}
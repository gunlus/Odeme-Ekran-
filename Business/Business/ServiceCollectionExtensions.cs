// Bu dosya API (WebApplication1) projesinin içindedir
using Microsoft.Extensions.DependencyInjection;
using Business; // Sadece Business katmanına bağımlıyız!

namespace WebApplication1.Extensions; 

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjeServisleri(this IServiceCollection services)
    {
        // Tüm DI (Dependency Injection) kayıt işlemlerini Business katmanına devrettik
        services.AddBusinessAndDataAccessServices();

        return services;
    }
}
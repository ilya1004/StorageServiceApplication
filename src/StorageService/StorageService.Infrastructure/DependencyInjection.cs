using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Abstractions.Services;
using StorageService.Infrastructure.Data;
using StorageService.Infrastructure.Repositories;
using StorageService.Infrastructure.Services;

namespace StorageService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(config =>
            config.UseSqlServer(configuration.GetConnectionString("MSSQLConnection")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();

        services.AddScoped<IDbStartupService, DbStartupService>();

        return services;
    }
}
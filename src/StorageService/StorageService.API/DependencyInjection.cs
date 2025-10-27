using System.Reflection;
using StorageService.API.Middlewares;

namespace StorageService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddAutoMapper(config =>
            config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddTransient<GlobalExceptionHandlingMiddleware>();

        return services;
    }
}
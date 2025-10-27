namespace StorageService.Domain.Abstractions.Services;

public interface IDbStartupService
{
    Task MakeMigrationsAsync();
    Task SeedDataAsync();
}
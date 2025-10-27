using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Domain.Abstractions.Services;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Services;

public class DbStartupService : IDbStartupService
{
    private readonly IServiceProvider _serviceProvider;
    private const int StorekeepersCount = 7;
    private const int DetailsCount = 25;

    public DbStartupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MakeMigrationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when applying database migrations", ex);
        }
    }

    public async Task SeedDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.Storekeepers.CountAsync() > StorekeepersCount ||
            await dbContext.Details.CountAsync() > DetailsCount)
        {
            return;
        }

        var storekeeperFaker = new Faker<Storekeeper>()
            .RuleFor(s => s.FullName, f =>
                {
                    var fullname = f.Name.FullName();
                    return fullname.Substring(0, Math.Min(fullname.Length, 200));
                })
            .RuleFor(s => s.IsDeleted, f => false);

        var storekeepers = storekeeperFaker.Generate(StorekeepersCount);
        await dbContext.Storekeepers.AddRangeAsync(storekeepers);
        await dbContext.SaveChangesAsync();

        var detailFaker = new Faker<Detail>()
            .RuleFor(d => d.NomenclatureCode, f => f.Random.Guid().ToString())
            .RuleFor(d => d.Name, f =>
                {
                    var name = f.Commerce.ProductName();
                    return name.Substring(0, Math.Min(name.Length, 200));
                })
            .RuleFor(d => d.Count, f => f.Random.Int(1, 100))
            .RuleFor(d => d.StorekeeperId, f => f.PickRandom(storekeepers).Id)
            .RuleFor(d => d.IsDeleted, f => false)
            .RuleFor(d => d.CreatedAtDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(d => d.DeletedAtDate, f => null);

        var details = detailFaker.Generate(DetailsCount);
        await dbContext.Details.AddRangeAsync(details);

        await dbContext.SaveChangesAsync();
    }
}
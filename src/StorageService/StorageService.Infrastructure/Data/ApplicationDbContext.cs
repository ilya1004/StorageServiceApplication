using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StorageService.Domain.Entities;

namespace StorageService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Detail> Details { get; set; }
    public DbSet<Storekeeper> Storekeepers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
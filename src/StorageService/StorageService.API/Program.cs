using Serilog;
using StorageService.API;
using StorageService.API.Middlewares;
using StorageService.Application;
using StorageService.Domain.Abstractions.Services;
using StorageService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddApi();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClientPolicy", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseCors("AngularClientPolicy");

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbStartupService = scope.ServiceProvider.GetRequiredService<IDbStartupService>();

    await dbStartupService.MakeMigrationsAsync();
    await dbStartupService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
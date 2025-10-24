using StorageService.API;
using StorageService.Application;
using StorageService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddApi();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
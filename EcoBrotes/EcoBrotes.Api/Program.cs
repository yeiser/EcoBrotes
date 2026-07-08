using EcoBrotes.Api.ApiHandlers;
using EcoBrotes.Api.Filters;
using EcoBrotes.Api.Middleware;
using EcoBrotes.Infrastructure.DataSource;
using EcoBrotes.Infrastructure.Extensions;
using FluentValidation;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Debugging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

/*builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(config.GetConnectionString("db"));
});*/

builder.Services.AddDbContext<DataContext>(opts =>
{
    var useInMemory = config.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        opts.UseInMemoryDatabase("InMemoryDb");
    }
    else
    {
        var connectionString = config.GetConnectionString("db");
        opts.UseNpgsql(connectionString);
    }
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>()
    .ForwardToPrometheus();

builder.Services.AddAutoMapper(Assembly.Load("EcoBrotes.Application"));

builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("EcoBrotes.Application"));
});

builder.Host.UseSerilog((_, loggerconfiguration) =>
    loggerconfiguration
        .WriteTo.Console()
        .WriteTo.File("logs.txt", Serilog.Events.LogEventLevel.Information));

SelfLog.Enable(Console.Error);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpMetrics();

app.UseMiddleware<AppExceptionHandlerMiddleware>();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.UseRouting().UseEndpoints(endpoint =>
{
    endpoint.MapMetrics();
});

app.MapGroup("/api/jornadas")
    .MapJornada()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Jornadas");

app.MapGroup("/api/especies")
    .MapEspecies()
    .WithTags("Especies");

app.MapGroup("/api/zonas")
    .MapZonas()
    .WithTags("Zonas");

app.Run();

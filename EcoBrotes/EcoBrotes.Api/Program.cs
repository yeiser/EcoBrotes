using EcoBrotes.Api.ApiHandlers;
using EcoBrotes.Api.Filters;
using EcoBrotes.Api.Middleware;
using EcoBrotes.Infrastructure.DataSource;
using EcoBrotes.Infrastructure.Extensions;
using FluentValidation;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Prometheus;
using Serilog;
using Serilog.Debugging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

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
        opts.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());
    }
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>()
    .ForwardToPrometheus();

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

// Apply pending EF Core migrations at startup when using a relational database.
// Skipped automatically for the in-memory provider used by integration tests.
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    if (dataContext.Database.IsRelational())
    {
        await dataContext.Database.MigrateAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de seguridad para headers
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        return Task.CompletedTask;
    });
    await next();
});

app.UseHttpMetrics();

app.UseMiddleware<AppExceptionHandlerMiddleware>();

app.UseRouting().UseEndpoints(endpoint =>
{
    endpoint.MapMetrics();
});

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapGroup("/api/jornadas")
    .MapJornada()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Jornadas");

app.MapGroup("/api/especies")
    .MapEspecies()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Especies");

app.MapGroup("/api/zonas")
    .MapZonas()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Zonas");

await app.RunAsync();

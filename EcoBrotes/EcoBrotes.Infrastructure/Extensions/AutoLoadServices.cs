using EcoBrotes.Application.Common;
using EcoBrotes.Application.Jornadas.Command.Factory;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Infrastructure.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace EcoBrotes.Infrastructure.Extensions;

public static class AutoLoadServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register all services as Scoped to share the same DbContext within a scope
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(JornadaFactory));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ReferentialIntegrityService>();

        var _services = AppDomain.CurrentDomain.GetAssemblies()
              .Where(assembly =>
              {
                  return assembly.FullName is null || assembly.FullName.Contains("Domain", StringComparison.OrdinalIgnoreCase);
              })
              .SelectMany(assembly => assembly.GetTypes())
              .Where(type => type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(DomainServiceAttribute)));

        var _repositories = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly =>
            {
                return assembly.FullName is null || assembly.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase);
            })
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(RepositoryAttribute)));

        foreach (var service in _services)
        {
            services.AddScoped(service);
        }

        foreach (var repository in _repositories)
        {
            Type typeInterface = repository.GetInterfaces().Single();
            services.AddScoped(typeInterface, repository);
        }

        return services;
    }
}

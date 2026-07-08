using EcoBrotes.Infrastructure.DataSource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoBrotes.Api.Tests;

class ApiApp : WebApplicationFactory<Program>
{
    readonly Guid _id;

    public Guid UserId => this._id;

    public ApiApp()
    {
        _id = Guid.NewGuid();
    }

    public IServiceProvider GetServices()
    {
        return Services;
    }

    public IServiceProvider GetServiceCollection()
    {
        return Services;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(svc =>
        {
            // Remove existing DbContext registration
            var descriptor = svc.SingleOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<DataContext>));
            if (descriptor != null)
            {
                svc.Remove(descriptor);
            }
            // Register DbContext as Scoped with unique database per ApiApp instance
            // Using a unique name with timestamp to ensure proper isolation
            var dbName = $"testdb_{_id}_{DateTimeOffset.UtcNow.Ticks}";
            svc.AddDbContext<DataContext>(opt =>
            {
                opt.UseInMemoryDatabase(dbName);
            }, ServiceLifetime.Scoped);
        });
    }
}

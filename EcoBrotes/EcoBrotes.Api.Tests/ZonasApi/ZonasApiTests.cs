using EcoBrotes.Infrastructure.DataSource;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;

namespace EcoBrotes.Api.Tests.ZonasApi;

public class ZonasApiTests
{
    static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetZonas_ReturnsAllZonas()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Seed zones
        var zona1 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Norte" };
        var zona2 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Sur" };
        var zona3 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Este" };

        ctx.Set<ZonaUrbanaEntity>().Add(zona1);
        ctx.Set<ZonaUrbanaEntity>().Add(zona2);
        ctx.Set<ZonaUrbanaEntity>().Add(zona3);
        await ctx.SaveChangesAsync();

        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/zonas");

        response.EnsureSuccessStatusCode();
        var zonas = JsonSerializer.Deserialize<List<ZonaDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(zonas);
        Assert.Equal(3, zonas.Count);
    }

    [Fact]
    public async Task GetZonas_ReturnsCorrectStructure()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Centro Comercial" };

        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        await ctx.SaveChangesAsync();

        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/zonas");

        response.EnsureSuccessStatusCode();
        var zonas = JsonSerializer.Deserialize<List<ZonaDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(zonas);
        Assert.Single(zonas);
        Assert.Equal(zona.Id, zonas[0].Id);
        Assert.Equal(zona.Name, zonas[0].Name);
    }

    [Fact]
    public async Task GetZonas_Empty_ReturnsEmptyList()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/zonas");

        response.EnsureSuccessStatusCode();
        var zonas = JsonSerializer.Deserialize<List<ZonaDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(zonas);
        Assert.Empty(zonas);
    }
}

using EcoBrotes.Infrastructure.DataSource;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Api.Tests.EspeciesApi;

public class EspeciesApiTests
{
    static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task GetEspecies_ReturnsAllEspecies()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Seed species
        var especie1 = new EspecieArboreaEntity 
        { 
            Id = Guid.NewGuid(), 
            Name = "Mangle Rojo", 
            ScientificName = "Rhizophora mangle", 
            MaxHeightMeters = 15m 
        };
        var especie2 = new EspecieArboreaEntity 
        { 
            Id = Guid.NewGuid(), 
            Name = "Coco", 
            ScientificName = "Cocos nucifera", 
            MaxHeightMeters = 20m 
        };
        var especie3 = new EspecieArboreaEntity 
        { 
            Id = Guid.NewGuid(), 
            Name = "Ceiba", 
            ScientificName = "Ceiba pentandra", 
            MaxHeightMeters = 30m 
        };

        ctx.Set<EspecieArboreaEntity>().Add(especie1);
        ctx.Set<EspecieArboreaEntity>().Add(especie2);
        ctx.Set<EspecieArboreaEntity>().Add(especie3);
        await ctx.SaveChangesAsync();

        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/especies");

        response.EnsureSuccessStatusCode();
        var especies = JsonSerializer.Deserialize<List<EspecieDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(especies);
        Assert.Equal(3, especies.Count);
    }

    [Fact]
    public async Task GetEspecies_ReturnsCorrectStructure()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        var especie = new EspecieArboreaEntity 
        { 
            Id = Guid.NewGuid(), 
            Name = "Guayacán", 
            ScientificName = "Handroanthus chrysanthus", 
            MaxHeightMeters = 18m 
        };

        ctx.Set<EspecieArboreaEntity>().Add(especie);
        await ctx.SaveChangesAsync();

        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/especies");

        response.EnsureSuccessStatusCode();
        var especies = JsonSerializer.Deserialize<List<EspecieDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(especies);
        Assert.Single(especies);
        Assert.Equal(especie.Id, especies[0].Id);
        Assert.Equal(especie.Name, especies[0].Name);
        Assert.Equal(especie.ScientificName, especies[0].ScientificName);
        Assert.Equal(especie.MaxHeightMeters, especies[0].MaxHeightMeters);
    }

    [Fact]
    public async Task GetEspecies_Empty_ReturnsEmptyList()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/especies");

        response.EnsureSuccessStatusCode();
        var especies = JsonSerializer.Deserialize<List<EspecieDto>>(
            await response.Content.ReadAsStringAsync(), _jsonOptions);

        Assert.NotNull(especies);
        Assert.Empty(especies);
    }
}

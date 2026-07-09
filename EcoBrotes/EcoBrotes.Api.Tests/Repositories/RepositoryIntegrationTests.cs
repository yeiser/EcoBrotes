using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Infrastructure.Adapters;
using EcoBrotes.Infrastructure.DataSource;
using EcoBrotes.Infrastructure.EspecieArborea.Adapters;
using EcoBrotes.Infrastructure.ZonaUrbana.Adapters;
using Microsoft.EntityFrameworkCore;

namespace EcoBrotes.Api.Tests.Repositories;

public class RepositoryIntegrationTests
{
    private static DataContext NewContext() =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static EspecieArboreaRepository EspecieRepo(DataContext ctx) =>
        new(new GenericRepository<EspecieArboreaEntity>(ctx), ctx);

    private static ZonaUrbanaRepository ZonaRepo(DataContext ctx) =>
        new(new GenericRepository<ZonaUrbanaEntity>(ctx), ctx);

    [Fact]
    public async Task Especie_Add_And_GetById_RoundTrips()
    {
        var ctx = NewContext();
        var repo = EspecieRepo(ctx);
        var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };

        await repo.AddAsync(especie);
        await ctx.SaveChangesAsync();

        var found = await repo.GetByIdAsync(especie.Id);
        Assert.NotNull(found);
        Assert.Equal("Roble", found.Name);
    }

    [Fact]
    public async Task Especie_GetByName_IsCaseInsensitive()
    {
        var ctx = NewContext();
        var repo = EspecieRepo(ctx);
        await repo.AddAsync(new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m });
        await ctx.SaveChangesAsync();

        var found = await repo.GetByNameAsync("ROBLE");
        Assert.NotNull(found);
        Assert.Equal("Roble", found.Name);

        Assert.Null(await repo.GetByNameAsync("Inexistente"));
    }

    [Fact]
    public async Task Especie_GetMany_ReturnsOnlyActive_And_CountMatches()
    {
        var ctx = NewContext();
        var repo = EspecieRepo(ctx);
        var activa = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
        var inactiva = new EspecieArboreaEntity { Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 15m };
        inactiva.Deactivate();
        await repo.AddAsync(activa);
        await repo.AddAsync(inactiva);
        await ctx.SaveChangesAsync();

        var many = await repo.GetManyAsync();
        Assert.Single(many);
        Assert.Equal("Roble", many.First().Name);
        Assert.Equal(1, await repo.GetCountAsync());
    }

    [Fact]
    public async Task Especie_Deactivate_SetsStateInactiva()
    {
        var ctx = NewContext();
        var repo = EspecieRepo(ctx);
        var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
        await repo.AddAsync(especie);
        await ctx.SaveChangesAsync();

        await repo.DeactivateAsync(especie.Id);

        var found = await repo.GetByIdAsync(especie.Id);
        Assert.Equal(EspecieState.Inactiva, found.State);
    }

    [Fact]
    public async Task Zona_Add_GetByName_And_GetMany_Work()
    {
        var ctx = NewContext();
        var repo = ZonaRepo(ctx);
        var activa = new ZonaUrbanaEntity { Name = "Zona Norte" };
        var inactiva = new ZonaUrbanaEntity { Name = "Zona Sur" };
        inactiva.Deactivate();
        await repo.AddAsync(activa);
        await repo.AddAsync(inactiva);
        await ctx.SaveChangesAsync();

        var byName = await repo.GetByNameAsync("zona norte");
        Assert.NotNull(byName);
        Assert.Equal("Zona Norte", byName.Name);

        var many = await repo.GetManyAsync();
        Assert.Single(many);
        Assert.Equal(1, await repo.GetCountAsync());
    }

    [Fact]
    public async Task Zona_Deactivate_SetsStateInactiva()
    {
        var ctx = NewContext();
        var repo = ZonaRepo(ctx);
        var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
        await repo.AddAsync(zona);
        await ctx.SaveChangesAsync();

        await repo.DeactivateAsync(zona.Id);

        var found = await repo.GetByIdAsync(zona.Id);
        Assert.Equal(ZonaState.Inactiva, found.State);
    }
}

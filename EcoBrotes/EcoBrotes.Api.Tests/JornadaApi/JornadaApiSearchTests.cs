using MediatR;
using EcoBrotes.Application.Jornadas.Query;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Infrastructure.DataSource;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoBrotes.Api.Tests.JornadaApi;

public class JornadaApiSearchTests
{
    /// <summary>
    /// Seeds zone, species, and jornadas directly in the shared DataContext.
    /// This avoids the scoped DbContext problem where each HTTP request
    /// gets a fresh in-memory database.
    /// </summary>
    private static async Task SeedJornadasAsync(IServiceScope scope, List<(string Nombre, int TreeMeta, int Cupo, bool IsFinalizada)> jornadasData)
    {
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Clean existing data to ensure test isolation
        var existingJornadas = await ctx.Set<JornadaReforestacion>().ToListAsync();
        if (existingJornadas.Count > 0)
        {
            ctx.Set<JornadaReforestacion>().RemoveRange(existingJornadas);
            await ctx.SaveChangesAsync();
        }

        // Remove existing entities to ensure clean state
        var existingEspecies = await ctx.Set<EspecieArboreaEntity>().ToListAsync();
        if (existingEspecies.Count > 0)
        {
            ctx.Set<EspecieArboreaEntity>().RemoveRange(existingEspecies);
            await ctx.SaveChangesAsync();
        }

        var existingZonas = await ctx.Set<ZonaUrbanaEntity>().ToListAsync();
        if (existingZonas.Count > 0)
        {
            ctx.Set<ZonaUrbanaEntity>().RemoveRange(existingZonas);
            await ctx.SaveChangesAsync();
        }

        // Create zones using object initializer pattern (no factory method exists)
        var zona1 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Norte" };
        var zona2 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Sur" };
        ctx.Set<ZonaUrbanaEntity>().Add(zona1);
        ctx.Set<ZonaUrbanaEntity>().Add(zona2);

        // Create species using object initializer pattern
        var especie1 = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus sylvestris", MaxHeightMeters = 20m };
        ctx.Set<EspecieArboreaEntity>().Add(especie1);
        await ctx.SaveChangesAsync();

        // Create jornadas directly in the DB
        var zonas = new[] { zona1, zona2 };
        foreach (var (nombre, treeMeta, cupo, isFinalizada) in jornadasData)
        {
            // Alternate between zones
            var zona = zonas[jornadasData.IndexOf((nombre, treeMeta, cupo, isFinalizada)) % 2];

            var jornada = new JornadaReforestacion
            {
                Id = Guid.NewGuid(),
                Name = nombre,
                Zona = zona,
                ZonaUrbanaId = zona.Id,
                ScheduledDate = DateTime.UtcNow.AddDays((jornadasData.IndexOf((nombre, treeMeta, cupo, isFinalizada)) + 1) * 7),
                TreeMeta = treeMeta,
                VolunteerCapacity = cupo,
                DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie1, EspecieArboreaId = especie1.Id, Quantity = treeMeta } },
                CodigoUnico = $"REF-{DateTime.UtcNow.Year}-{jornadasData.IndexOf((nombre, treeMeta, cupo, isFinalizada)) + 1:000}"
            };

            ctx.Set<JornadaReforestacion>().Add(jornada);

            if (isFinalizada)
            {
                jornada.Finalizar();
            }
        }

        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task GetJornadas_NoFilters_ReturnsAll()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Seed 2 jornadas
        await SeedJornadasAsync(scope, [
            ("Jornada Norte", 10, 10, false),
            ("Jornada Sur", 5, 5, false),
        ]);

        var query = new GetJornadasQuery { Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Equal(2, list.Count);
        // RB-03: Order by scheduled date ascending
        Assert.True(list[0].ScheduledDate <= list[1].ScheduledDate);
    }

    [Fact]
    public async Task GetJornadas_FilterByZona_ReturnsFiltered()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Create zones
        var zona1 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona 1" };
        var zona2 = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona 2" };
        ctx.Set<ZonaUrbanaEntity>().Add(zona1);
        ctx.Set<ZonaUrbanaEntity>().Add(zona2);

        // Create species
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Create jornadas in different zones
        var jornada1 = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Zona 1",
            Zona = zona1,
            ZonaUrbanaId = zona1.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(14),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-001"
        };

        var jornada2 = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Zona 2",
            Zona = zona2,
            ZonaUrbanaId = zona2.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(21),
            TreeMeta = 5,
            VolunteerCapacity = 5,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 5 } },
            CodigoUnico = "REF-2026-002"
        };

        ctx.Set<JornadaReforestacion>().Add(jornada1);
        ctx.Set<JornadaReforestacion>().Add(jornada2);
        await ctx.SaveChangesAsync();

        var query = new GetJornadasQuery { ZonaId = zona1.Id, Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Single(list);
        Assert.Equal("Jornada Zona 1", list[0].Name);
    }

    [Fact]
    public async Task GetJornadas_FilterByState_ReturnsFiltered()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Create zone and species
        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona A" };
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Jornada abierta
        var jornadaAbierta = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Abierta",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(14),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-001"
        };

        // Jornada finalizada
        var jornadaFinalizada = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Finalizada",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(10),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-002"
        };
        jornadaFinalizada.Finalizar();

        ctx.Set<JornadaReforestacion>().Add(jornadaAbierta);
        ctx.Set<JornadaReforestacion>().Add(jornadaFinalizada);
        await ctx.SaveChangesAsync();

        var query = new GetJornadasQuery { Estados = new[] { "Finalizada" }, Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Single(list);
        Assert.Equal("Finalizada", list[0].Estado);
    }

    [Fact]
    public async Task GetJornadas_FilterByDateRange_ReturnsFiltered()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Create zone and species
        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona B" };
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Jornada en fecha cercana (15 días)
        var jornadaCercana = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Marzo",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(15),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-001"
        };

        // Jornada en fecha lejana (60 días)
        var jornadaLejana = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Junio",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(60),
            TreeMeta = 5,
            VolunteerCapacity = 5,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 5 } },
            CodigoUnico = "REF-2026-002"
        };

        ctx.Set<JornadaReforestacion>().Add(jornadaCercana);
        ctx.Set<JornadaReforestacion>().Add(jornadaLejana);
        await ctx.SaveChangesAsync();

        var fechaDesde = DateTime.UtcNow.AddDays(30);
        var fechaHasta = DateTime.UtcNow.AddDays(90);
        var query = new GetJornadasQuery { FechaDesde = fechaDesde, FechaHasta = fechaHasta, Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Single(list);
        Assert.Equal("Jornada Junio", list[0].Name);
    }

    [Fact]
    public async Task GetJornadas_CombinedFilters_ReturnsFiltered()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Create zone and species
        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona C" };
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Jornada abierta cercana (15 días)
        var jornadaAbiertaCercana = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Abierta Marzo",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(15),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-001"
        };

        // Jornada finalizada cercana (10 días)
        var jornadaFinalizadaCercana = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Finalizada Marzo",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(10),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } },
            CodigoUnico = "REF-2026-002"
        };
        jornadaFinalizadaCercana.Finalizar();

        // Jornada finalizada lejana (60 días)
        var jornadaFinalizadaLejana = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Finalizada Junio",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(60),
            TreeMeta = 5,
            VolunteerCapacity = 5,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 5 } },
            CodigoUnico = "REF-2026-003"
        };
        jornadaFinalizadaLejana.Finalizar();

        ctx.Set<JornadaReforestacion>().Add(jornadaAbiertaCercana);
        ctx.Set<JornadaReforestacion>().Add(jornadaFinalizadaCercana);
        ctx.Set<JornadaReforestacion>().Add(jornadaFinalizadaLejana);
        await ctx.SaveChangesAsync();

        var fechaDesde = DateTime.UtcNow.AddDays(30);
        var fechaHasta = DateTime.UtcNow.AddDays(90);
        var query = new GetJornadasQuery { Estados = new[] { "Finalizada" }, FechaDesde = fechaDesde, FechaHasta = fechaHasta, Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Single(list);
        Assert.Equal("Jornada Finalizada Junio", list[0].Name);
    }

    [Fact]
    public async Task GetJornadas_NoResults_ReturnsEmpty()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        var query = new GetJornadasQuery { FechaDesde = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), FechaHasta = new DateTime(2020, 1, 31, 0, 0, 0, DateTimeKind.Utc), Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Empty(list);
    }

    [Fact]
    public async Task GetJornadas_InvalidDateRange_ReturnsBadRequest()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var request = await client.GetAsync("/api/jornadas/?page=1&pageSize=20&fechaDesde=2026-12-31&fechaHasta=2026-01-01");

        Assert.Equal("BadRequest", request.StatusCode.ToString());
    }

    [Fact]
    public async Task GetJornadas_Pagination_ReturnsPaginated()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Clean ALL existing data to ensure complete test isolation
        var existingJornadas = await ctx.Set<JornadaReforestacion>().ToListAsync();
        if (existingJornadas.Count > 0)
        {
            ctx.Set<JornadaReforestacion>().RemoveRange(existingJornadas);
            await ctx.SaveChangesAsync();
        }

        var existingEspecies = await ctx.Set<EspecieArboreaEntity>().ToListAsync();
        if (existingEspecies.Count > 0)
        {
            ctx.Set<EspecieArboreaEntity>().RemoveRange(existingEspecies);
            await ctx.SaveChangesAsync();
        }

        var existingZonas = await ctx.Set<ZonaUrbanaEntity>().ToListAsync();
        if (existingZonas.Count > 0)
        {
            ctx.Set<ZonaUrbanaEntity>().RemoveRange(existingZonas);
            await ctx.SaveChangesAsync();
        }

        // Create zone and species
        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona D" };
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Create 5 jornadas
        for (int i = 1; i <= 5; i++)
        {
            var jornada = new JornadaReforestacion
            {
                Id = Guid.NewGuid(),
                Name = $"Jornada {i}",
                Zona = zona,
                ZonaUrbanaId = zona.Id,
                ScheduledDate = DateTime.UtcNow.AddDays(i * 7),
                TreeMeta = 10,
                VolunteerCapacity = 10,
                DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } }
            };
            ctx.Set<JornadaReforestacion>().Add(jornada);
        }

        await ctx.SaveChangesAsync();

        // Use HTTP client to ensure same scope is used for handler
        var client = webApp.CreateClient();
        var response = await client.GetAsync("/api/jornadas/?page=1&pageSize=2");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var jornadas = System.Text.Json.JsonSerializer.Deserialize<List<JornadaSummaryDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(jornadas);
        Assert.Equal(2, jornadas.Count);
        
        // Verify pagination headers
        Assert.Equal("5", response.Headers.GetValues("X-Total-Count").First());
        Assert.Equal("1", response.Headers.GetValues("X-Page").First());
        Assert.Equal("2", response.Headers.GetValues("X-Page-Size").First());
    }

    [Fact]
    public async Task GetJornadas_OcupacionPct_Calculated()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        // Create zone and species
        var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona E" };
        var especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20m };
        ctx.Set<ZonaUrbanaEntity>().Add(zona);
        ctx.Set<EspecieArboreaEntity>().Add(especie);

        // Jornada con cupo de 10 voluntarios, 0 inscritos
        var jornada = new JornadaReforestacion
        {
            Id = Guid.NewGuid(),
            Name = "Jornada Vacía",
            Zona = zona,
            ZonaUrbanaId = zona.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(14),
            TreeMeta = 10,
            VolunteerCapacity = 10,
            DetalleArboles = new List<DetalleArbolEntity> { new DetalleArbolEntity { Especie = especie, EspecieArboreaId = especie.Id, Quantity = 10 } }
        };
        ctx.Set<JornadaReforestacion>().Add(jornada);

        await ctx.SaveChangesAsync();

        var query = new GetJornadasQuery { Page = 1, PageSize = 20 };
        var result = await mediator.Send(query);
        var list = result.Items.ToList();

        Assert.Single(list);
        Assert.Equal(0m, list[0].OcupacionPct);
    }
}

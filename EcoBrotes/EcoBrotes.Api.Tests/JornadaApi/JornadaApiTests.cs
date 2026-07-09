using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Ports;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EcoBrotes.Api.Tests.JornadaApi;

public class JornadaApiTests
{
    static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task PostJornadasSuccess()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().Build();
        var especie1 = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();
        var especie2 = new EspecieArboreaDataBuilder().WithName("Roble").WithScientificName("Quercus").WithMaxHeightMeters(15m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie1);
        await CreateEspecie(scope, especie2);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var command = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Reforestación Norte")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie1.Id, 6),
                new DetalleEspecieCommand(especie2.Id, 4)
            ])
            .Build();

        var client = webApp.CreateClient();
        var request = await client.PostAsJsonAsync("/api/jornadas/", command);

        request.EnsureSuccessStatusCode();
        var jornadaId = JsonSerializer.Deserialize<Guid>(await request.Content.ReadAsStringAsync(), _jsonOptions);
        Assert.NotEqual(Guid.Empty, jornadaId);
    }

    [Fact]
    public async Task PostJornadas_InconsistentMeta_Rejected()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().Build();
        var especie = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var command = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Reforestación Norte")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 7)
            ])
            .Build();

        var client = webApp.CreateClient();
        var request = await client.PostAsJsonAsync("/api/jornadas/", command);

        Assert.Equal("BadRequest", request.StatusCode.ToString());
    }

    [Fact]
    public async Task PostJornadas_InvalidDate_Rejected()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var command = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(Guid.NewGuid())
            .WithName("Jornada Reforestación Norte")
            .WithScheduledDate(DateTime.UtcNow.AddDays(3))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(Guid.NewGuid(), 10)
            ])
            .Build();

        var request = await client.PostAsJsonAsync("/api/jornadas/", command);

        Assert.Equal("BadRequest", request.StatusCode.ToString());
    }

    [Fact]
    public async Task PostJornadas_VolunteerRatioViolated_Rejected()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().Build();
        var especie = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var command = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Reforestación Norte")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(1)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 10)
            ])
            .Build();

        var client = webApp.CreateClient();
        var request = await client.PostAsJsonAsync("/api/jornadas/", command);

        Assert.Equal("BadRequest", request.StatusCode.ToString());
    }

    [Fact]
    public async Task PostJornadas_DuplicateSpecies_Rejected()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().Build();
        var especie = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var command = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Reforestación Norte")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 5),
                new DetalleEspecieCommand(especie.Id, 5)
            ])
            .Build();

        var client = webApp.CreateClient();
        var request = await client.PostAsJsonAsync("/api/jornadas/", command);

        Assert.Equal("BadRequest", request.StatusCode.ToString());
    }

    static async Task CreateZona(IServiceScope scope, ZonaUrbanaEntity zona)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ZonaUrbanaEntity>>();
        await repository.AddAsync(zona);
    }

    static async Task CreateEspecie(IServiceScope scope, EspecieArboreaEntity especie)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<EspecieArboreaEntity>>();
        await repository.AddAsync(especie);
    }
}

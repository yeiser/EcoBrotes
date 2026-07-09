using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace EcoBrotes.Api.Tests.JornadaApi;

public class UpdateJornadaApiTests
{
    [Fact]
    public async Task PutJornadasSuccess()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().WithName("Zona Test").Build();
        var especie1 = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();
        var especie2 = new EspecieArboreaDataBuilder().WithName("Roble").WithScientificName("Quercus").WithMaxHeightMeters(15m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie1);
        await CreateEspecie(scope, especie2);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        // Create jornada via command handler
        var createCommand = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Original")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie1.Id, 6),
                new DetalleEspecieCommand(especie2.Id, 4)
            ])
            .Build();

        var jornadaId = await mediator.Send(createCommand);
        Assert.NotEqual(Guid.Empty, jornadaId);

        // Now update the jornada via command handler (within same scope)
        var updateCommand = new UpdateJornadaCommand(
            jornadaId,
            zona.Id,
            "Jornada Actualizada",
            DateTime.UtcNow.AddDays(20),
            15,
            3,
            new List<DetalleEspecieCommand>
            {
                new DetalleEspecieCommand(especie1.Id, 10),
                new DetalleEspecieCommand(especie2.Id, 5)
            }
        );

        await mediator.Send(updateCommand);
    }

    [Fact]
    public async Task PutJornadas_NotFound_ReturnsBadRequest()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var updateCommand = new UpdateJornadaCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Jornada Actualizada",
            DateTime.UtcNow.AddDays(20),
            10,
            2,
            new List<DetalleEspecieCommand>
            {
                new DetalleEspecieCommand(Guid.NewGuid(), 10)
            }
        );

        await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(updateCommand));
    }

    [Fact]
    public async Task PutJornadas_AlreadyFinalizada_Rejected()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var zona = new ZonaUrbanaDataBuilder().WithName("Zona Test").Build();
        var especie = new EspecieArboreaDataBuilder().WithName("Pino").WithScientificName("Pinus").WithMaxHeightMeters(20m).Build();

        await CreateZona(scope, zona);
        await CreateEspecie(scope, especie);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        // Create a jornada via command handler
        var createCommand = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Finalizada")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 10)
            ])
            .Build();

        var jornadaId = await mediator.Send(createCommand);

        // Manually update jornada state to Finalizada
        var jornadaRepo = scope.ServiceProvider.GetRequiredService<IJornadaReforestacionRepository>();
        var jornada = await jornadaRepo.GetByIdAsync(jornadaId);
        Assert.NotNull(jornada);
        jornada.Finalizar();
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        // Try to update the jornada - should be rejected if not in ConvocatoriaAbierta state
        var updateCommand = new UpdateJornadaCommand(
            jornadaId,
            zona.Id,
            "Jornada Actualizada",
            DateTime.UtcNow.AddDays(20),
            10,
            2,
            new List<DetalleEspecieCommand>
            {
                new DetalleEspecieCommand(especie.Id, 10)
            }
        );

        await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(updateCommand));
    }

    private static async Task CreateZona(IServiceScope scope, ZonaUrbanaEntity zona)
    {
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<ZonaUrbanaEntity>>();
        await repo.AddAsync(zona);
    }

    private static async Task CreateEspecie(IServiceScope scope, EspecieArboreaEntity especie)
    {
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<EspecieArboreaEntity>>();
        await repo.AddAsync(especie);
    }
}

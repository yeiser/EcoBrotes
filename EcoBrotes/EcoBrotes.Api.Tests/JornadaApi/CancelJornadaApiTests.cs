using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace EcoBrotes.Api.Tests.JornadaApi;

public class CancelJornadaApiTests
{
    [Fact]
    public async Task DeleteJornadasSuccess()
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

        var createCommand = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada a Cancelar")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 10)
            ])
            .Build();

        var jornadaId = await mediator.Send(createCommand);
        Assert.NotEqual(Guid.Empty, jornadaId);

        var cancelCommand = new CancelJornadaCommand(jornadaId);
        await mediator.Send(cancelCommand);
    }

    [Fact]
    public async Task DeleteJornadas_NotFound_ReturnsBadRequest()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var cancelCommand = new CancelJornadaCommand(Guid.NewGuid());
        await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(cancelCommand));
    }

    [Fact]
    public async Task DeleteJornadas_Finalizada_Rejected()
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

        var jornadaRepo = scope.ServiceProvider.GetRequiredService<IJornadaReforestacionRepository>();
        var jornada = await jornadaRepo.GetByIdAsync(jornadaId);
        Assert.NotNull(jornada);
        jornada.Finalizar();
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);

        var cancelCommand = new CancelJornadaCommand(jornadaId);
        await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(cancelCommand));
    }

    [Fact]
    public async Task DeleteJornadas_AlreadyCancelada_Rejected()
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

        var createCommand = new CreateJornadaCommandBuilder()
            .WithZonaUrbanaId(zona.Id)
            .WithName("Jornada Ya Cancelada")
            .WithScheduledDate(DateTime.UtcNow.AddDays(14))
            .WithTreeMeta(10)
            .WithVolunteerCapacity(2)
            .WithDetalleEspecies([
                new DetalleEspecieCommand(especie.Id, 10)
            ])
            .Build();

        var jornadaId = await mediator.Send(createCommand);

        var cancelCommand1 = new CancelJornadaCommand(jornadaId);
        await mediator.Send(cancelCommand1);

        var cancelCommand2 = new CancelJornadaCommand(jornadaId);
        await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(cancelCommand2));
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

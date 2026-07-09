using EcoBrotes.Application.Jornadas.Command.Factory;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.JornadasReforestacion.Service;
using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    internal class CreateJornadaHandler(
        JornadaFactory jornadaFactory,
        GenerateCodigoUnicoService generateCodigoUnicoService,
        SaveJornadaService saveJornadaService,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateJornadaCommand, Guid>
    {
        public async Task<Guid> Handle(CreateJornadaCommand request, CancellationToken cancellationToken)
        {
            // Force UTC to avoid Npgsql timestamp with time zone errors
            var scheduledDate = request.ScheduledDate;
            if (scheduledDate.Kind != DateTimeKind.Utc)
            {
                scheduledDate = DateTime.SpecifyKind(scheduledDate, DateTimeKind.Utc);
            }

            // Create a new command with the UTC date
            var utcRequest = new CreateJornadaCommand(
                request.ZonaUrbanaId,
                request.Name,
                scheduledDate,
                request.TreeMeta,
                request.VolunteerCapacity,
                request.DetalleEspecies);

            var codigoUnico = await generateCodigoUnicoService.ExecuteAsync();
            var jornada = await jornadaFactory.CreateAsync(utcRequest, codigoUnico);
            var jornadaId = await saveJornadaService.ExecuteAsync(jornada);
            await unitOfWork.SaveAsync(cancellationToken);
            return jornadaId;
        }
    }
}

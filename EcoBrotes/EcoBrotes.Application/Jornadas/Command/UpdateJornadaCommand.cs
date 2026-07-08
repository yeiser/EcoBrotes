using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    public record UpdateJornadaCommand(
        Guid Id,
        Guid ZonaUrbanaId,
        string Name,
        DateTime ScheduledDate,
        int TreeMeta,
        int VolunteerCapacity,
        IEnumerable<DetalleEspecieCommand> DetalleEspecies
    ) : IRequest<Unit>, IJornadaCommand;
}

using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    public record CreateJornadaCommand(
        Guid ZonaUrbanaId,
        string Name,
        DateTime ScheduledDate,
        int TreeMeta,
        int VolunteerCapacity,
        IEnumerable<DetalleEspecieCommand> DetalleEspecies
    ) : IRequest<Guid>, IJornadaCommand;
}

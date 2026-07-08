using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    public record CancelJornadaCommand(Guid Id) : IRequest<Unit>;
}

using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    public record UpdateZonaCommand(
        Guid Id,
        string Name
    ) : IRequest<Unit>;
}

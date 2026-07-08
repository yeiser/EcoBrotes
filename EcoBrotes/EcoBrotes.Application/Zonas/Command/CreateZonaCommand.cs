using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    public record CreateZonaCommand(
        string Name
    ) : IRequest<Guid>;
}

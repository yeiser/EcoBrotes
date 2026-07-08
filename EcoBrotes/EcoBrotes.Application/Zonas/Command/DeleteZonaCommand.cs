using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    public record DeleteZonaCommand(Guid Id) : IRequest<Unit>;
}

using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    public record DeleteEspecieCommand(Guid Id) : IRequest<Unit>;
}

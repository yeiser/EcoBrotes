using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    public record UpdateEspecieCommand(
        Guid Id,
        string Name,
        string ScientificName,
        decimal MaxHeightMeters
    ) : IRequest<Unit>;
}

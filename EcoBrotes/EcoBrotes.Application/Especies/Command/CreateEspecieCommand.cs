using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    public record CreateEspecieCommand(
        string Name,
        string ScientificName,
        decimal MaxHeightMeters
    ) : IRequest<Guid>;
}

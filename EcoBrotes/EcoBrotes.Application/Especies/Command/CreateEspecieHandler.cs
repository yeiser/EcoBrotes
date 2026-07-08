using EcoBrotes.Application.Common;
using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    internal class CreateEspecieHandler(
        IEspecieArboreaRepository especieRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateEspecieCommand, Guid>
    {
        public async Task<Guid> Handle(CreateEspecieCommand request, CancellationToken cancellationToken)
        {
            // Validate no duplicate species name
            await DuplicateValidationService.ValidateNoDuplicateNameAsync(
                () => especieRepository.GetByNameAsync(request.Name),
                "una especie",
                request.Name);

            var especie = new EspecieArboreaEntity
            {
                Name = request.Name,
                ScientificName = request.ScientificName,
                MaxHeightMeters = request.MaxHeightMeters
            };

            await especieRepository.AddAsync(especie);
            await unitOfWork.SaveAsync(cancellationToken);
            return especie.Id;
        }
    }
}

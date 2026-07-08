using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    internal class UpdateEspecieHandler(
        IRepository<EspecieArboreaEntity> especieRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateEspecieCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateEspecieCommand request, CancellationToken cancellationToken)
        {
            var especie = await especieRepository.GetByIdAsync(request.Id);
            especie.ValidateNull($"La especie con id {request.Id} no existe.");

            // RB-06: Cannot update a deactivated species
            if (especie.State == EspecieState.Inactiva)
            {
                throw new CoreBusinessException($"La especie con id {request.Id} está desactivada y no puede ser actualizada.");
            }

            especie.Name = request.Name;
            especie.ScientificName = request.ScientificName;
            especie.MaxHeightMeters = request.MaxHeightMeters;

            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

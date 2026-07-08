using EcoBrotes.Application.Common;
using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using MediatR;

namespace EcoBrotes.Application.Especies.Command
{
    internal class DeleteEspecieHandler(
        IEspecieArboreaRepository especieRepository,
        IJornadaReforestacionRepository jornadaRepository,
        ReferentialIntegrityService referentialIntegrity,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteEspecieCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteEspecieCommand request, CancellationToken cancellationToken)
        {
            var especie = await especieRepository.GetByIdAsync(request.Id);
            especie.ValidateNull($"La especie con id {request.Id} no existe.");

            // RB-06: Validate state - check if species is already inactive
            if (especie.State == EspecieState.Inactiva)
            {
                throw new CoreBusinessException($"La especie con id {request.Id} ya se encuentra desactivada.");
            }

            // RB-09: Validate referential integrity using shared service
            await referentialIntegrity.ValidateNoActiveReferencesAsync(
                () => jornadaRepository.GetByEspecieArboreaIdAsync(request.Id),
                "la especie",
                request.Id);

            // RB-06: Soft delete - deactivate instead of physical delete
            especie.Deactivate();

            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

using EcoBrotes.Application.Common;
using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    internal class DeleteZonaHandler(
        IZonaUrbanaRepository zonaRepository,
        IJornadaReforestacionRepository jornadaRepository,
        ReferentialIntegrityService referentialIntegrity,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteZonaCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteZonaCommand request, CancellationToken cancellationToken)
        {
            var zona = await zonaRepository.GetByIdAsync(request.Id);
            zona.ValidateNull($"La zona urbana con id {request.Id} no existe.");

            // RB-06: Validate state - check if zone is already inactive
            if (zona.State == ZonaState.Inactiva)
            {
                throw new CoreBusinessException($"La zona urbana con id {request.Id} ya se encuentra desactivada.");
            }

            // RB-09: Validate referential integrity using shared service
            await referentialIntegrity.ValidateNoActiveReferencesAsync(
                () => jornadaRepository.GetByZonaIdAsync(request.Id),
                "la zona urbana",
                request.Id);

            // RB-06: Soft delete - deactivate instead of physical delete
            zona.Deactivate();

            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

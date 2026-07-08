using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    internal class UpdateZonaHandler(
        IRepository<ZonaUrbanaEntity> zonaRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateZonaCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateZonaCommand request, CancellationToken cancellationToken)
        {
            var zona = await zonaRepository.GetByIdAsync(request.Id);
            zona.ValidateNull($"La zona urbana con id {request.Id} no existe.");

            // RB-06: Cannot update a deactivated zone
            if (zona.State == ZonaState.Inactiva)
            {
                throw new CoreBusinessException($"La zona urbana con id {request.Id} está desactivada y no puede ser actualizada.");
            }

            zona.Name = request.Name;

            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

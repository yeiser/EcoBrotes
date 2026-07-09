using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Jornadas.Command.Factory;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    internal class UpdateJornadaHandler(
        IJornadaReforestacionRepository jornadaRepository,
        JornadaFactory jornadaFactory,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateJornadaCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateJornadaCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener y validar existencia de la jornada (con sus detalles para poder reemplazarlos)
            var jornada = await jornadaRepository.GetByIdAsync(request.Id, "DetalleArboles");
            if (jornada == null)
            {
                throw new CoreBusinessException($"La jornada con id {request.Id} no existe.");
            }

            // 2. Validar estado (regla de negocio)
            if (jornada.State != JornadaState.ConvocatoriaAbierta)
            {
                throw new CoreBusinessException("Solo se pueden editar jornadas en estado 'Convocatoria Abierta'.");
            }

            // 3. Delegar validación de zonas y especies al Factory
            var detalleArboles = await jornadaFactory.ValidateUpdateAsync(
                request.ZonaUrbanaId, request.DetalleEspecies);

            // 4. Actualizar campos de la jornada
            jornada.Name = request.Name;
            jornada.ScheduledDate = request.ScheduledDate;
            jornada.TreeMeta = request.TreeMeta;
            jornada.VolunteerCapacity = request.VolunteerCapacity;

            // 5. Actualizar los detalles de árboles
            jornada.DetalleArboles.Clear();
            foreach (var detalle in detalleArboles)
            {
                jornada.DetalleArboles.Add(detalle);
            }

            // 6. Validar consistencia de metas (regla de negocio del dominio)
            jornada.ValidateConsistency();

            // 7. Persistir cambios
            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

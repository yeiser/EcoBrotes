using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    internal class CancelJornadaHandler(
        IJornadaReforestacionRepository jornadaRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CancelJornadaCommand, Unit>
    {
        public async Task<Unit> Handle(CancelJornadaCommand request, CancellationToken cancellationToken)
        {
            var jornada = await jornadaRepository.GetByIdAsync(request.Id);
            if (jornada == null)
            {
                throw new CoreBusinessException($"La jornada con id {request.Id} no existe.");
            }

            // Regla de negocio: Solo se puede cancelar si no está "Finalizada"
            // La entidad ya valida esto en el método Cancel()
            jornada.Cancel();

            await unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

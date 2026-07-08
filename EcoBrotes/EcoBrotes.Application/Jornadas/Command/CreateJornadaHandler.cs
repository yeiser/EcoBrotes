using EcoBrotes.Application.Jornadas.Command.Factory;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.JornadasReforestacion.Service;
using MediatR;

namespace EcoBrotes.Application.Jornadas.Command
{
    internal class CreateJornadaHandler(
        JornadaFactory jornadaFactory,
        GenerateCodigoUnicoService generateCodigoUnicoService,
        SaveJornadaService saveJornadaService,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateJornadaCommand, Guid>
    {
        public async Task<Guid> Handle(CreateJornadaCommand request, CancellationToken cancellationToken)
        {
            var codigoUnico = await generateCodigoUnicoService.ExecuteAsync();
            var jornada = await jornadaFactory.CreateAsync(request, codigoUnico);
            var jornadaId = await saveJornadaService.ExecuteAsync(jornada);
            await unitOfWork.SaveAsync(cancellationToken);
            return jornadaId;
        }
    }
}

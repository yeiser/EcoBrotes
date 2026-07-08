using EcoBrotes.Application.Common;
using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using MediatR;

namespace EcoBrotes.Application.Zonas.Command
{
    internal class CreateZonaHandler(
        IZonaUrbanaRepository zonaRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateZonaCommand, Guid>
    {
        public async Task<Guid> Handle(CreateZonaCommand request, CancellationToken cancellationToken)
        {
            // Validate no duplicate zone name
            await DuplicateValidationService.ValidateNoDuplicateNameAsync(
                () => zonaRepository.GetByNameAsync(request.Name),
                "una zona urbana",
                request.Name);

            var zona = new ZonaUrbanaEntity
            {
                Name = request.Name
            };

            await zonaRepository.AddAsync(zona);
            await unitOfWork.SaveAsync(cancellationToken);
            return zona.Id;
        }
    }
}

using EcoBrotes.Application.Common;
using EcoBrotes.Application.Zonas.Query;
using EcoBrotes.Domain.ZonaUrbana.Port;
using ZonaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using MediatR;

namespace EcoBrotes.Application.Zonas.Query
{
    public class GetZonaByIdHandler(
        IZonaUrbanaRepository zonaRepository)
        : IRequestHandler<GetZonaByIdQuery, ZonaDto>
    {
        public async Task<ZonaDto> Handle(GetZonaByIdQuery request, CancellationToken cancellationToken)
        {
            var zona = await zonaRepository.GetByIdAsync(request.Id);
            return EntityMapper.MapEntity(
                zona,
                "La zona urbana con id " + request.Id,
                z => new ZonaDto
                {
                    Id = z.Id,
                    Name = z.Name
                });
        }
    }
}

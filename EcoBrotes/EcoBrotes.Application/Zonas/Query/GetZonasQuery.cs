using MediatR;
using EcoBrotes.Application.Common;
using EcoBrotes.Domain.ZonaUrbana.Port;
using ZonaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;

namespace EcoBrotes.Application.Zonas.Query
{
    public class GetZonasQuery : IRequest<IEnumerable<ZonaDto>>
    {
    }

    public class GetZonasHandler(
        IZonaUrbanaRepository zonaRepository)
        : IRequestHandler<GetZonasQuery, IEnumerable<ZonaDto>>
    {
        public async Task<IEnumerable<ZonaDto>> Handle(GetZonasQuery request, CancellationToken cancellationToken)
        {
            var zonas = await zonaRepository.GetManyAsync();
            return EntityMapper.MapEntities(
                zonas,
                z => new ZonaDto
                {
                    Id = z.Id,
                    Name = z.Name
                });
        }
    }
}

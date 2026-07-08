using MediatR;
using EcoBrotes.Application.Common;
using EcoBrotes.Domain.EspecieArborea.Port;
using EspecieEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Application.Especies.Query
{
    public class GetEspeciesQuery : IRequest<IEnumerable<EspecieDto>>
    {
    }

    public class GetEspeciesHandler(
        IEspecieArboreaRepository especieRepository)
        : IRequestHandler<GetEspeciesQuery, IEnumerable<EspecieDto>>
    {
        public async Task<IEnumerable<EspecieDto>> Handle(GetEspeciesQuery request, CancellationToken cancellationToken)
        {
            var especies = await especieRepository.GetManyAsync();
            return EntityMapper.MapEntities(
                especies,
                e => new EspecieDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    ScientificName = e.ScientificName,
                    MaxHeightMeters = e.MaxHeightMeters
                });
        }
    }
}

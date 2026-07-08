using EcoBrotes.Application.Common;
using EcoBrotes.Application.Especies.Query;
using EcoBrotes.Domain.EspecieArborea.Port;
using EspecieEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using MediatR;

namespace EcoBrotes.Application.Especies.Query
{
    public class GetEspecieByIdHandler(
        IEspecieArboreaRepository especieRepository)
        : IRequestHandler<GetEspecieByIdQuery, EspecieDto>
    {
        public async Task<EspecieDto> Handle(GetEspecieByIdQuery request, CancellationToken cancellationToken)
        {
            var especie = await especieRepository.GetByIdAsync(request.Id);
            return EntityMapper.MapEntity(
                especie,
                "La especie con id " + request.Id,
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

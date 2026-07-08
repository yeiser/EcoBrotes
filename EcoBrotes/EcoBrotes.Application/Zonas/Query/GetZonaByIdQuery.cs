using MediatR;

namespace EcoBrotes.Application.Zonas.Query
{
    public class GetZonaByIdQuery : IRequest<ZonaDto>
    {
        public Guid Id { get; set; }
    }
}

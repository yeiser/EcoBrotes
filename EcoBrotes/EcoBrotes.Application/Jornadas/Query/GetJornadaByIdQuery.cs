using MediatR;

namespace EcoBrotes.Application.Jornadas.Query
{
    public class GetJornadaByIdQuery : IRequest<JornadaDetailDto>
    {
        public Guid Id { get; set; }
    }
}

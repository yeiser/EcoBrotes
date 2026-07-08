using MediatR;

namespace EcoBrotes.Application.Especies.Query
{
    public class GetEspecieByIdQuery : IRequest<EspecieDto>
    {
        public Guid Id { get; set; }
    }
}

using EcoBrotes.Application.Jornadas.Query;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class GetJornadasRequestValidator : AbstractValidator<GetJornadasRequest>
{
    public GetJornadasRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .GreaterThan(0);

        RuleFor(request => request.FechaHasta)
            .GreaterThan(request => request.FechaDesde)
            .When(request => request.FechaDesde.HasValue && request.FechaHasta.HasValue)
            .WithMessage("Rango de fechas erróneo. La fecha de inicio de la búsqueda no puede ser mayor que la fecha fin.");
    }
}

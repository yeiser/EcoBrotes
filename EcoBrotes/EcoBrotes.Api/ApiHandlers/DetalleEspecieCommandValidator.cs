using EcoBrotes.Application.Jornadas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class DetalleEspecieCommandValidator : AbstractValidator<DetalleEspecieCommand>
{
    public DetalleEspecieCommandValidator()
    {
        RuleFor(command => command.EspecieArboreaId)
            .NotEmpty();

        RuleFor(command => command.Quantity)
            .GreaterThan(0);
    }
}

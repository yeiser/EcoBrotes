using EcoBrotes.Application.Especies.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class UpdateEspecieCommandValidator : AbstractValidator<UpdateEspecieCommand>
{
    public UpdateEspecieCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 150);

        RuleFor(command => command.ScientificName)
            .NotEmpty();

        RuleFor(command => command.MaxHeightMeters)
            .GreaterThan(0);
    }
}

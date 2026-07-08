using EcoBrotes.Application.Especies.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class CreateEspecieCommandValidator : AbstractValidator<CreateEspecieCommand>
{
    public CreateEspecieCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 150);

        RuleFor(command => command.ScientificName)
            .NotEmpty();

        RuleFor(command => command.MaxHeightMeters)
            .GreaterThan(0);
    }
}

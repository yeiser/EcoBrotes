using EcoBrotes.Application.Zonas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class CreateZonaCommandValidator : AbstractValidator<CreateZonaCommand>
{
    public CreateZonaCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 100);
    }
}

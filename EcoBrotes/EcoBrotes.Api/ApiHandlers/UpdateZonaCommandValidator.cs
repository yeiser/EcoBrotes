using EcoBrotes.Application.Zonas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class UpdateZonaCommandValidator : AbstractValidator<UpdateZonaCommand>
{
    public UpdateZonaCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 100);
    }
}

using EcoBrotes.Application.Zonas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class DeleteZonaCommandValidator : AbstractValidator<DeleteZonaCommand>
{
    public DeleteZonaCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}

using EcoBrotes.Api.Validation;
using EcoBrotes.Application.Jornadas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class CreateJornadaCommandValidator : JornadaCommandValidator<CreateJornadaCommand>
{
    public CreateJornadaCommandValidator()
    {
        RuleFor(command => command.ZonaUrbanaId)
            .NotEmpty();
    }
}

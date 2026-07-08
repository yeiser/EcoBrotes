using EcoBrotes.Api.Validation;
using EcoBrotes.Application.Jornadas.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class UpdateJornadaCommandValidator : JornadaCommandValidator<UpdateJornadaCommand>
{
    public UpdateJornadaCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}

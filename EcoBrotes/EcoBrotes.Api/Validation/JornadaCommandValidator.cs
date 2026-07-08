using EcoBrotes.Api.ApiHandlers;
using EcoBrotes.Application.Jornadas.Command;
using FluentValidation;

namespace EcoBrotes.Api.Validation;

/// <summary>
/// Base validator with common rules for Jornada commands (Create and Update).
/// Reduces code duplication by centralizing shared validation rules.
/// </summary>
public class JornadaCommandValidator<TCommand> : AbstractValidator<TCommand> where TCommand : class, IJornadaCommand
{
    public JornadaCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(command => command.ScheduledDate)
            .GreaterThan(DateTime.UtcNow.AddDays(7));

        RuleFor(command => command.TreeMeta)
            .GreaterThan(0);

        RuleFor(command => command.VolunteerCapacity)
            .GreaterThan(0);

        RuleFor(command => command.DetalleEspecies)
            .NotNull()
            .NotEmpty();

        RuleForEach(command => command.DetalleEspecies)
            .SetValidator(new DetalleEspecieCommandValidator());
    }
}

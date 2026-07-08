using EcoBrotes.Application.Especies.Command;
using FluentValidation;

namespace EcoBrotes.Api.ApiHandlers;

public class DeleteEspecieCommandValidator : AbstractValidator<DeleteEspecieCommand>
{
    public DeleteEspecieCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}

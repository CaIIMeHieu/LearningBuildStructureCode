using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.UserCases.V1.Deck.Validator;

public class CreateDeckValidator : AbstractValidator<CommandSource.CreateDeckCommand>
{
    public CreateDeckValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}

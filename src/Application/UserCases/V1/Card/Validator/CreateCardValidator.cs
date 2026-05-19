using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.UserCases.V1.Card.Validator;

public class CreateCardValidator : AbstractValidator<CommandSource.CreateCardCommand>
{
    public CreateCardValidator()
    {
        RuleFor(x => x.DeckId).NotEmpty();
        RuleFor(x => x.Question).NotEmpty();
        RuleFor(x => x.Answer).NotEmpty();
    }
}

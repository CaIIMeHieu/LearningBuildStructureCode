using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Constants;
using FluentValidation;

namespace Application.UserCases.V1.Card.Validator;

public class ReviewCardValidator : AbstractValidator<CommandSource.ReviewCardCommand>
{
    private static readonly string[] AllowedQualities = { QualityCard.Again, QualityCard.Hard, QualityCard.Good, QualityCard.Easy };

    public ReviewCardValidator()
    {
        RuleFor(x => x.CardId).NotEmpty();

        RuleFor(x => x.Quality)
            .NotEmpty()
            .Must(x => AllowedQualities.Contains(x))
            .WithMessage($"Quality must be one of the following: {string.Join(", ", AllowedQualities)}");
    }
}

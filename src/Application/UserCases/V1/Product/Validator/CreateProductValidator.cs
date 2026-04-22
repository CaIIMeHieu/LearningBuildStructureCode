using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.UserCases.V1.Product.Validator;

public class CreateProductValidator : AbstractValidator<CommandSource.CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");
    }
}

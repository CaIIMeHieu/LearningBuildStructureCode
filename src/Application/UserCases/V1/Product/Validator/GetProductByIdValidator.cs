using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.UserCases.V1.Product.Validator;

public class GetProductByIdValidator : AbstractValidator<QuerySource.GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

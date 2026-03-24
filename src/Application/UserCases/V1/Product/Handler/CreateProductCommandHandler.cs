using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using static Application.UserCases.V1.Product.Command;

namespace Application.UserCases.V1.Product.Handler;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

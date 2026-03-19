using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Shared;
using MediatR;

namespace Contract.Abstractions.Message;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;

namespace Application.UserCases.V1.Query;

internal class GetProductQueryHandler : IQueryHandler<GetProductQuery, GetProductResponse>
{
    public Task<Result<GetProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using static Application.UserCases.V1.Product.Response;

namespace Application.UserCases.V1.Product;

public static class QuerySource
{
    public record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
    public record GetProductsQuery() : IQuery<List<ProductResponse>>;
}

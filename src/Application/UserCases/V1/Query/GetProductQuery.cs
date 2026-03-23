using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Application.UserCases.V1.Query;

public class GetProductQuery : IQuery<GetProductResponse>
{
    public string Name { get; set; }
}

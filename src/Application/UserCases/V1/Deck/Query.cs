using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;

namespace Application.UserCases.V1.Deck;

public class QuerySource
{
    public record GetUserDecksQuery(PagedRequest PagedRequest) : IQuery<PageResultT<Domain.Entities.Deck>>;
}

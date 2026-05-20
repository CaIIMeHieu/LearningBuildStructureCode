using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;

namespace Application.UserCases.V1.Card;

public class QuerySource
{
    public record GetDueCardsQuery( Guid DeckId, Guid userId, PagedRequest PagedRequest ) : IQuery<List<Response.CardResponse>>;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using MediatR;
using Microsoft.VisualBasic;

namespace Application.UserCases.V1.Card.Handler.Query;

public class GetDueCardsQueryHandler : IQueryHandler<QuerySource.GetDueCardsQuery, List<Response.CardResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepository;
    private readonly IRepositoryBase<Domain.Entities.Deck, Guid> _deckRepository;
    private readonly IMapper _mapper;
    public GetDueCardsQueryHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository, IRepositoryBase<Domain.Entities.Deck, Guid> deckRepository, IMapper mapper)
    {
        _cardRepository = cardRepository;
        _deckRepository = deckRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.CardResponse>>> Handle(QuerySource.GetDueCardsQuery request, CancellationToken cancellationToken)
    {
        var deckCards = await _deckRepository.FindSingleAsync(d => d.Id == request.DeckId && d.OwnerId == request.UserId, cancellationToken) ?? throw new ArgumentException("Deck not found or access denied");
        var dueCards = await _cardRepository.FindAllPagedAsync(request.PagedRequest, c => c.DeckId == request.DeckId && c.RecallDate < DateTime.UtcNow);
        var results = _mapper.Map<List<Response.CardResponse>>(dueCards);
        return results;
    }
} 

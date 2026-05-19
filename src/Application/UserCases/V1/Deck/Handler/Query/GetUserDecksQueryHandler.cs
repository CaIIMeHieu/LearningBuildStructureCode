using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using static Application.UserCases.V1.Deck.QuerySource;

namespace Application.UserCases.V1.Deck.Handler.Handler;

public class GetUserDecksQueryHandler : IQueryHandler<QuerySource.GetUserDecksQuery, PageResultT<Domain.Entities.Deck>>
{
    private readonly IRepositoryBase<Domain.Entities.Deck, Guid> _deckRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserDecksQueryHandler(
        IRepositoryBase<Domain.Entities.Deck, Guid> deckRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _deckRepository = deckRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    // Handler
    public async Task<Result<PageResultT<Domain.Entities.Deck>>> Handle(
        GetUserDecksQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User
            .Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Result.Failure<PageResultT<Domain.Entities.Deck>>(
                Error.Unauthorized("Unauthorized", "User is not authenticated."));

        var ownerGuid = Guid.Parse(userId);

        var pagedDecks = await _deckRepository.FindAllPagedAsync(
            request.PagedRequest,
            d => d.OwnerId == ownerGuid,
            cancellationToken);

        //var result = new PageResultT<DeckResponse>
        //{
        //    Items = pagedDecks.Items
        //    .Select(d => new DeckResponse(d.Id, d.Name.Value, d.Description))
        //    .ToList(),
        //    TotalCount = pagedDecks.TotalCount,
        //    PageIndex = pagedDecks.PageIndex,
        //    PageSize = pagedDecks.PageSize
        //};

        return Result.Success(pagedDecks);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Application.UserCases.V1.Deck.Handler.Command;

public class CreateDeckCommandHandler : ICommandHandler<CommandSource.CreateDeckCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Deck, Guid> _deckRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDeckCommandHandler(IRepositoryBase<Domain.Entities.Deck, Guid> deckRepository, IHttpContextAccessor httpContextAccessor)
    {
        _deckRepository = deckRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(CommandSource.CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if( string.IsNullOrEmpty(userId) )
        {
            return Result.Failure(Error.Unauthorized("Unauthorized", "User is not authenticated."));
        }
        var deck = Domain.Entities.Deck.Create(request.Name, request.Description, Guid.Parse(userId));
        _deckRepository.Add(deck);
        return Result.Success();
    }
}

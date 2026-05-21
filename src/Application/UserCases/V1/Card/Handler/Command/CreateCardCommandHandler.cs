using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Application.UserCases.V1.Card.Handler.Command;

public class CreateCardCommandHandler : ICommandHandler<CommandSource.CreateCardCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Card,Guid> _cardRepository;
    private readonly IRepositoryBase<Domain.Entities.Deck, Guid> _deckRepository;
    public CreateCardCommandHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository, IRepositoryBase<Domain.Entities.Deck, Guid> deckRepository)
    {
        _cardRepository = cardRepository;
        _deckRepository = deckRepository;
    }
    public async Task<Result> Handle(CommandSource.CreateCardCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await _deckRepository.FindByIdAsync(request.DeckId, cancellationToken);
        if (deckExists == null)
        {
            return Result.Failure(Error.NotFound("Deck not found", "Deck not found"));
        }
        var card = Domain.Entities.Card.Create(request.DeckId, request.Question, request.Answer, request.Note, request.OwnerId);
        _cardRepository.Add(card);
        return Result.Success();
    }
}

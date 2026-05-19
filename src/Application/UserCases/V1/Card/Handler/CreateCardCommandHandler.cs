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

namespace Application.UserCases.V1.Card.Handler;

public class CreateCardCommandHandler : ICommandHandler<CommandSource.CreateCardCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Card,Guid> _cardRepository;
    public CreateCardCommandHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository)
    {
        _cardRepository = cardRepository;
    }
    public async Task<Result> Handle(CommandSource.CreateCardCommand request, CancellationToken cancellationToken)
    {
        var deckExists = await _cardRepository.FindByIdAsync(request.DeckId, cancellationToken);
        if (deckExists == null)
        {
            return Result.Failure(Error.NotFound("Deck not found", "Deck not found"));
        }
        var card = Domain.Entities.Card.Create(request.DeckId, request.Question, request.Answer, request.Note);        
        _cardRepository.Add(card);
        return Result.Success();
    }
}

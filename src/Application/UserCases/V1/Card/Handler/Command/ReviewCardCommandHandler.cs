using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;

namespace Application.UserCases.V1.Card.Handler.Command;

public class ReviewCardCommandHandler : ICommandHandler<CommandSource.ReviewCardCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepository;
    public ReviewCardCommandHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result> Handle(CommandSource.ReviewCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.FindByIdAsync(request.CardId);
        if( card == null )
        {
            return Result.Failure(Error.NotFound("Card not found","Card not found"));
        }
        card.Review(request.Quality);
        var reviewLog = Domain.Entities.ReviewLog.Create(card.Id, request.Quality);
        _cardRepository.Update(card);
        return Result.Success();
    }
}

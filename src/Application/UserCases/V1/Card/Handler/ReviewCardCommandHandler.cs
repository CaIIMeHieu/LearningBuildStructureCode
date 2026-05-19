using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;

namespace Application.UserCases.V1.Card.Handler;

public class ReviewCardCommandHandler : ICommandHandler<CommandSource.ReviewCardCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepository;
    private readonly IRepositoryBase<Domain.Entities.ReviewLog, Guid> _reviewLogRepository;
    public ReviewCardCommandHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository, IRepositoryBase<Domain.Entities.ReviewLog, Guid> reviewLogRepository)
    {
        _cardRepository = cardRepository;
        _reviewLogRepository = reviewLogRepository;
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
        _reviewLogRepository.Add(reviewLog);
        return Result.Success();
    }
}

using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;

namespace Application.UserCases.V1.Card.Handler.Command;

public class ReviewCardCommandHandler : ICommandHandler<CommandSource.ReviewCardCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepository;

    public ReviewCardCommandHandler(IRepositoryBase<Domain.Entities.Card, Guid> cardRepository)
        => _cardRepository = cardRepository;

    public async Task<Result> Handle(CommandSource.ReviewCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.FindByIdAsync(request.CardId, cancellationToken);
        if (card == null || card.OwnerId != request.OwnerId)
            return Result.Failure(Error.NotFound("Card.NotFound", "Card not found"));

        card.Review(request.Quality);
        _cardRepository.Update(card);
        // query xem còn due cards ngày hôm nay không
        var hasDueCards = await _cardRepository.FindSingleAsync(c => c.OwnerId == request.OwnerId && c.RecallDate <= DateTime.UtcNow);
        return Result.Success();
    }
}

using Contract.Abstractions.Message;
using Domain.Abstractions;
using Domain.Entities;
using static Domain.Entities.Card;

namespace Application.UserCases.V1.Card.Handler.Event;

public class CardReviewedEventHandler : IDomainHandler<CardReviewedEvent>
{
    private readonly IRepositoryBase<ReviewLog, Guid> _reviewLogRepository;

    public CardReviewedEventHandler(IRepositoryBase<ReviewLog, Guid> reviewLogRepository)
        => _reviewLogRepository = reviewLogRepository;

    public Task Handle(CardReviewedEvent notification, CancellationToken cancellationToken)
    {
        var reviewLog = ReviewLog.Create(notification.Id, notification.Quality);
        _reviewLogRepository.Add(reviewLog);
        Console.WriteLine($"Card reviewed: {notification.Id} by owner {notification.OwnerId}");
        return Task.CompletedTask;
    }
}

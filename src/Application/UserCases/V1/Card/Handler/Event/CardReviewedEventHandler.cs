using Contract.Abstractions.Message;
using Domain.Abstractions;
using Domain.Entities;
using static Domain.Entities.Card;

namespace Application.UserCases.V1.Card.Handler.Event;

public class CardReviewedEventHandler : IDomainHandler<CardReviewedEvent>
{
    private readonly IRepositoryBase<ReviewLog, Guid> _reviewLogRepo;
    private readonly IRepositoryBase<Domain.Entities.UserProfile, Guid> _userProfileRepo;

    public CardReviewedEventHandler(
        IRepositoryBase<ReviewLog, Guid> reviewLogRepo,
        IRepositoryBase<Domain.Entities.UserProfile, Guid> userProfileRepo)
    {
        _reviewLogRepo = reviewLogRepo;
        _userProfileRepo = userProfileRepo;
    }

    public async Task Handle(CardReviewedEvent notification, CancellationToken cancellationToken)
    {
        // Save review log (đã có sẵn)
        var reviewLog = ReviewLog.Create(notification.Id, notification.Quality, notification.OwnerId);
        _reviewLogRepo.Add(reviewLog);

        // Increment counter cho user
        var profile = await _userProfileRepo.FindByIdAsync(
            notification.OwnerId, cancellationToken);
        if (profile is not null)
        {
            profile.IncrementReviewCount();
            _userProfileRepo.Update(profile);
        }
    }
}

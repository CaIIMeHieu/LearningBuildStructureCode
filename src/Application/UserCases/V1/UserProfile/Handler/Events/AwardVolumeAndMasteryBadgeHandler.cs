using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Domain.Abstractions;
using Domain.Enumerations;

namespace Application.UserCases.V1.UserProfile.Handler.Events;

public class AwardVolumeAndMasteryBadgeHandler : IDomainHandler<Domain.Entities.UserProfile.AllCardsReviewedEvent>
{
    private readonly IRepositoryBase<Domain.Entities.UserProfile, Guid> _userProfileRepo;
    private readonly IRepositoryBase<Domain.Entities.ReviewLog, Guid> _reviewLogRepo;
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepo;

    public AwardVolumeAndMasteryBadgeHandler(
        IRepositoryBase<Domain.Entities.UserProfile, Guid> userProfileRepo,
        IRepositoryBase<Domain.Entities.ReviewLog, Guid> reviewLogRepo,
        IRepositoryBase<Domain.Entities.Card, Guid> cardRepo)
    {
        _userProfileRepo = userProfileRepo;
        _reviewLogRepo = reviewLogRepo;
        _cardRepo = cardRepo;
    }

    public async Task Handle(
        Domain.Entities.UserProfile.AllCardsReviewedEvent notification,
        CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepo.FindByIdAsync(
            notification.UserId, cancellationToken);
        if (profile is null)
            return;

        // === Volume badges ===

        if (profile.TotalReviews >= 100)
            profile.AwardBadge(BadgeType.Volume100);
        if (profile.TotalReviews >= 500)
            profile.AwardBadge(BadgeType.Volume500);
        if (profile.TotalReviews >= 1000)
            profile.AwardBadge(BadgeType.Volume1000);

        // === Mastery badge ===
        // Có >= 10 cards với Interval > 30
        var masteryCount = _cardRepo.FindAll(
            c => c.OwnerId == notification.UserId && c.Interval > 30);

        if (masteryCount.ToList().Count >= 10)
            profile.AwardBadge(BadgeType.Mastery);

        _userProfileRepo.Update(profile);
    }
}

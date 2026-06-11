using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using static Application.UserCases.V1.UserProfile.QuerySource;
using static Application.UserCases.V1.UserProfile.Response;

namespace Application.UserCases.V1.UserProfile.Handler.Query;

public class GetMyProfileQueryHandler : IQueryHandler<GetMyProfileQuery, ProfileResponse>
{
    private readonly IRepositoryBase<Domain.Entities.UserProfile, Guid> _profileRepo;
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepo;

    public GetMyProfileQueryHandler(
        IRepositoryBase<Domain.Entities.UserProfile, Guid> profileRepo,
        IRepositoryBase<Domain.Entities.Card, Guid> cardRepo)
    {
        _profileRepo = profileRepo;
        _cardRepo = cardRepo;
    }

    public async Task<Result<ProfileResponse>> Handle(
        GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.FindByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result.Failure<ProfileResponse>(
                Error.NotFound("Profile.NotFound", "Profile not found"));

        // === Lazy recalculate streak ===
        // Check missed days từ LastReviewedDate+1 đến hôm qua
        // V1: dùng UTC, sau này refactor theo TimeZoneId
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var lastReviewed = profile.Streak.LastReviewedDate;

        if (lastReviewed.HasValue)
        {
            var fromDate = lastReviewed.Value.AddDays(1);
            var toDate = today.AddDays(-1);

            if (fromDate <= toDate)
            {
                var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue);
                var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue);

                var missedCard = await _cardRepo.FindSingleAsync(
                    c => c.OwnerId == request.UserId
                      && c.RecallDate >= fromDateTime
                      && c.RecallDate <= toDateTime,
                    cancellationToken);

                if (missedCard != null)
                {
                    profile.RecalculateStreak(hasMissedDays: true);
                    _profileRepo.Update(profile);
                }
            }
        }

        // Build response
        var response = new ProfileResponse(
            StreakCurrent: profile.Streak.Current,
            StreakLongest: profile.Streak.Longest,
            LastReviewedDate: profile.Streak.LastReviewedDate,
            TotalReviews: profile.TotalReviews,
            Badges: profile.Badges
                .Select(b => new BadgeResponse(b.BadgeType.ToString(), b.EarnedAt))
                .OrderByDescending(b => b.EarnedAt)
                .ToList());

        return Result.Success(response);
    }
}

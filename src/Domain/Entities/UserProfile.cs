using System.Net.WebSockets;
using Contract.Abstractions.Message;
using Domain.Abstractions.Entities;
using Domain.Constants;
using Domain.Enumerations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Domain.Entities;

public class UserProfile : AggregateRoot
{
    public string TimeZoneId { get; private set; } = "UTC";
    public DateTime  CreatedAt { get; private set; }
    public Streak Streak { get; private set; } = Streak.New();
    public int TotalReviews { get; private set; } = 0;
    // Kiểm soát việc người dùng có thể tương tác với Badge, chỉ có thể chỉnh sửa thông qua UserProfile
    private readonly List<UserBadge> _badges = new();
    public IReadOnlyList<UserBadge> Badges => _badges.AsReadOnly();
    protected UserProfile() { }

    private UserProfile(Guid userId, string timeZoneId) : base(userId)
    {
        TimeZoneId = timeZoneId;
        CreatedAt = DateTime.UtcNow;
    }

    public static UserProfile Create(Guid userId, string timeZoneId = "UTC")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.");
        return new UserProfile(userId, timeZoneId);
    }

    // record review của user khi user review hết card, cập nhật streak và raise event tương ứng
    public void RecordReview(DateOnly reviewDate, bool hasMissedDays)
    {
        var oldStreak = Streak.Current;
        Streak = Streak.RecordReview(reviewDate, hasMissedDays);
        if (Streak.Current > oldStreak) RaiseDomainEvent(new StreakIncreasedEvent(Id, Streak.Current));
        CheckStreakBadges();
    }

    // tính toán streak khi user vào màn homeScreen, nếu ko bị miss ngày hoặc streak = 0 thì không cần reset streak và raise event
    public void RecalculateStreak( bool hasMissedDays )
    {
        if (!hasMissedDays)
            return;
        if (Streak.Current == 0)
            return;
        Streak = Streak.Reset();
        RaiseDomainEvent(new StreakBrokenEvent(Id, Streak.Current));
    }

    // hàm chốt badge với badgeType là volumn hoặc mastery ( vì phải query trong db chứ không check trực tiếp như Streak được )
    public void AwardBadge(BadgeType badgeType)
    {
        TryAwardBadge(badgeType, true);
    }

    // hàm kiểm tra khi mà User review xong card và tăng streak
    private void CheckStreakBadges()
    {
        TryAwardBadge(BadgeType.Streak3, Streak.Current >= 3);
        TryAwardBadge(BadgeType.Streak7, Streak.Current >= 7);
        TryAwardBadge(BadgeType.Streak30, Streak.Current >= 30);
        TryAwardBadge(BadgeType.Streak100, Streak.Current >= 100);
    }

    // hàm kiếm badge, nếu đúng điều kiện và user chưa có badge đó thì tạo badge mới và raise events. Nhận vào condition(bool)
    private void TryAwardBadge(BadgeType badgeType, bool condition)
    {
        if (!condition)
            return;
        // Kiểm tra nếu user đã có badge này rồi thì không award nữa
        if (_badges.Any(b => b.BadgeType == badgeType))
            return;
        var newBadge = UserBadge.Create(Id, badgeType);
        _badges.Add(newBadge);
        RaiseDomainEvent(new BadgeEarnedEvent(Id, badgeType));
    }

    public void IncrementReviewCount()
    {
        TotalReviews++;
    }

    // Raise từ UserProfile khi streak tăng
    public record StreakIncreasedEvent(Guid Id, int NewStreak) : IDomain;

    // Raise từ UserProfile khi streak bị reset
    public record StreakBrokenEvent(Guid Id, int CurrentStreak) : IDomain;

    // Raise từ UserProfile khi earn badge mới
    public record BadgeEarnedEvent(Guid Id, BadgeType BadgeType) : IDomain;
    // Raise từ ReviewCardCommandHandler khi user review hết due cards
    public record AllCardsReviewedEvent(Guid Id, Guid UserId, DateOnly ReviewDate) : IDomain;
}






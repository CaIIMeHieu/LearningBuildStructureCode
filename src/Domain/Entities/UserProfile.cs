using Contract.Abstractions.Message;
using Domain.Abstractions.Entities;
using Domain.Constants;
using Domain.Enumerations;

namespace Domain.Entities;

public class UserProfile : AggregateRoot
{
    public string UserId { get; private set; }
    public string TimeZoneId { get; private set; } = "UTC";
    public DateTime  CreatedAt { get; private set; }
    public Streak Streak { get; private set; } = Streak.New();

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
    // tính toán streak khi user vào màn homeScreen, nếu ko bị miss ngày hoặc streak = 0 thì không cần reset streak và raise event
    // hàm kiếm badge, nếu đúng điều kiện và user chưa có badge đó thì tạo badge mới và raise events. Nhận vào condition(bool)

    // Raise từ UserProfile khi streak tăng
    public record StreakIncreasedEvent(Guid Id, int NewStreak) : IDomain;

    // Raise từ UserProfile khi streak bị reset
    public record StreakBrokenEvent(Guid Id, int CurrentStreak) : IDomain;

    // Raise từ UserProfile khi earn badge mới
    public record BadgeEarnedEvent(Guid Id, BadgeType BadgeType) : IDomain;
    // Raise từ ReviewCardCommandHandler khi user review hết due cards
    public record AllCardsReviewedEvent(Guid Id, Guid UserId, DateOnly ReviewDate) : IDomain;
}






using Domain.Abstractions.Entities;
using Domain.Constants;

namespace Domain.Entities;

public class ReviewLog : DomainEntity<Guid>
{
    public Guid CardId { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public string Quality { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    protected ReviewLog() { }

    private ReviewLog(Guid id, Guid cardId, DateTime reviewDate, string quality, Guid ownerId  ) : base(id)
    {
        CardId = cardId;
        ReviewDate = reviewDate;
        Quality = quality;
        OwnerId = ownerId;
    }

    public static ReviewLog Create(Guid cardId, string quality, Guid ownerId)
    {
        string[] standardQuality = { QualityCard.Again, QualityCard.Hard, QualityCard.Good, QualityCard.Easy };
        if (cardId == Guid.Empty)
            throw new ArgumentException("Card ID cannot be empty.");
        if (ownerId == Guid.Empty)
            throw new ArgumentException("Owner ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(quality))
            throw new ArgumentException("Quality cannot be empty or null.");
        if (!standardQuality.Contains(quality))
            throw new ArgumentException("Invalid quality value.");
        return new ReviewLog(Guid.NewGuid(), cardId, DateTime.UtcNow, quality, ownerId);
    }
}

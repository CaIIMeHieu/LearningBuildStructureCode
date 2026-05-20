using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Constants;

namespace Domain.Entities;

public class ReviewLog
{
    public Guid Id { get; private set; }
    public Guid CardId { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public string Quality { get; private set; } = string.Empty;
    protected ReviewLog() { }
    private ReviewLog(Guid id, Guid cardId, DateTime reviewDate, string quality)
    {
        Id = id;
        CardId = cardId;
        ReviewDate = reviewDate;
        Quality = quality;
    }
    public static ReviewLog Create(Guid cardId, string quality)
    {
        string[] standardQuality = { QualityCard.Again, QualityCard.Hard, QualityCard.Good, QualityCard.Easy };        
        if (cardId == Guid.Empty)
            throw new ArgumentException("Card ID cannot be empty.");            
        if (string.IsNullOrWhiteSpace(quality))
            throw new ArgumentException("Quality cannot be empty or null.");
        else if (!standardQuality.Contains(quality))
        {
            throw new ArgumentException("Invalid quality value.");
        }
        return new ReviewLog(Guid.NewGuid(), cardId, DateTime.UtcNow, quality);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions.Entities;
using Domain.Constants;

namespace Domain.Entities;

public class Card : AggregateRoot
{
    public Guid DeckId { get; private set; }
    public string Question { get; private set; } = string.Empty;
    public string Answer { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public DateTime RecallDate { get; private set; } = DateTime.UtcNow.AddDays(1);
    public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
    public int Interval { get; private set; } = 1;
    public double EaseFactor { get; private set; } = 2.5;
    public int Repetitions { get; private set; } = 0;
    private readonly List<ReviewLog> _reviewLogs = new ();
    // tách riêng field và property để đảm bảo chỉ có thể đọc từ bên ngoài và không thể sửa đổi trực tiếp danh sách review logs từ bên ngoài lớp
    public IReadOnlyList<ReviewLog> ReviewLogs => _reviewLogs.AsReadOnly();

    protected Card() { }
    private Card(Guid id, Guid deckId, string question, string answer, string? note): base(id)
    {
        DeckId = deckId;
        Question = question;
        Answer = answer;
        Note = note;
    }

    public static Card Create(Guid deckId, string question, string answer, string? note)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("Deck ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(question))
            throw new ArgumentException("Question cannot be empty or null.");
        if (string.IsNullOrWhiteSpace(answer))
            throw new ArgumentException("Answer cannot be empty or null.");
        return new Card(Guid.NewGuid(), deckId, question, answer, note);
    }

    public void Review(string quality)
    {
        string[] standardQuality = { QualityCard.Again, QualityCard.Hard, QualityCard.Good, QualityCard.Easy };
        if (!standardQuality.Contains(quality))
        {
            throw new ArgumentException("Invalid quality value.");
        }
        if (quality == QualityCard.Again)
        {
            Interval = 1;
            EaseFactor = Math.Max(1.3, EaseFactor - 0.2);
            Repetitions = 0;
        }
        else if (quality == QualityCard.Hard)
        {
            Interval = Math.Max(1, Interval / 2);
            EaseFactor = Math.Max(1.3, EaseFactor - 0.15);
            Repetitions++;
        }
        else if (quality == QualityCard.Good)
        {
            Interval = (int)(Interval * EaseFactor);
            Repetitions++;
        }
        else if (quality == QualityCard.Easy)
        {
            Interval = (int)(Interval * EaseFactor * 1.3);
            EaseFactor = Math.Min(2.5, EaseFactor + 0.15);
            Repetitions++;
        }
        RecallDate = DateTime.Now.AddDays(Interval);
        _reviewLogs.Add(ReviewLog.Create(Id, quality));
    }
}

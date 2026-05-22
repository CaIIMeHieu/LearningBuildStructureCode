using Domain.Constants;
using Domain.Entities;
using FluentAssertions;
using static Domain.Entities.Card;

namespace Test.Unit;

public class CardAggregateTests
{
    private static Card CreateValidCard() =>
        Card.Create(Guid.NewGuid(), "What is DDD?", "Domain-Driven Design", null, Guid.NewGuid());

    // ── Card.Create() ─────────────────────────────────────────────────────────

    [Fact]
    public void Create_HappyPath_ShouldReturnCardWithCorrectValues()
    {
        var deckId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var card = Card.Create(deckId, "Question?", "Answer", "Note", ownerId);

        card.Id.Should().NotBe(Guid.Empty);
        card.DeckId.Should().Be(deckId);
        card.OwnerId.Should().Be(ownerId);
        card.Question.Should().Be("Question?");
        card.Answer.Should().Be("Answer");
        card.Note.Should().Be("Note");
        card.Interval.Should().Be(1);
        card.EaseFactor.Should().Be(2.5);
        card.Repetitions.Should().Be(0);
    }

    [Fact]
    public void Create_EmptyDeckId_ShouldThrowArgumentException()
    {
        var act = () => Card.Create(Guid.Empty, "Q", "A", null, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_EmptyOwnerId_ShouldThrowArgumentException()
    {
        var act = () => Card.Create(Guid.NewGuid(), "Q", "A", null, Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhitespaceQuestion_ShouldThrowArgumentException(string? question)
    {
        var act = () => Card.Create(Guid.NewGuid(), question!, "A", null, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhitespaceAnswer_ShouldThrowArgumentException(string? answer)
    {
        var act = () => Card.Create(Guid.NewGuid(), "Q", answer!, null, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldRaiseCardCreatedEvent()
    {
        var deckId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var card = Card.Create(deckId, "Q", "A", null, ownerId);

        card.DomainEvents.Should().ContainSingle(e => e is CardCreatedEvent)
            .Which.Should().BeOfType<CardCreatedEvent>()
            .Which.Should().Match<CardCreatedEvent>(e =>
                e.Id == card.Id && e.DeckId == deckId && e.OwnerId == ownerId);
    }

    // ── Card.Review() ─────────────────────────────────────────────────────────

    [Fact]
    public void Review_InvalidQuality_ShouldThrowArgumentException()
    {
        var card = CreateValidCard();

        var act = () => card.Review("Perfect");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Review_Again_ShouldResetIntervalAndRepetitionsAndDecreaseEaseFactor()
    {
        var card = CreateValidCard();
        var previousEaseFactor = card.EaseFactor;

        card.Review(QualityCard.Again);

        card.Interval.Should().Be(1);
        card.Repetitions.Should().Be(0);
        card.EaseFactor.Should().Be(Math.Max(1.3, previousEaseFactor - 0.2));
    }

    [Fact]
    public void Review_Hard_ShouldHalveIntervalAndIncrementRepetitionsAndDecreaseEaseFactor()
    {
        var card = CreateValidCard();
        card.Review(QualityCard.Good); // Interval → 2
        var intervalBefore = card.Interval;
        var easeFactorBefore = card.EaseFactor;
        var repetitionsBefore = card.Repetitions;

        card.Review(QualityCard.Hard);

        card.Interval.Should().Be(Math.Max(1, intervalBefore / 2));
        card.Repetitions.Should().Be(repetitionsBefore + 1);
        card.EaseFactor.Should().Be(Math.Max(1.3, easeFactorBefore - 0.15));
    }

    [Fact]
    public void Review_Good_ShouldMultiplyIntervalByEaseFactorAndIncrementRepetitions()
    {
        var card = CreateValidCard();
        var intervalBefore = card.Interval;
        var easeFactorBefore = card.EaseFactor;

        card.Review(QualityCard.Good);

        card.Interval.Should().Be(Math.Max(1, (int)(intervalBefore * easeFactorBefore)));
        card.Repetitions.Should().Be(1);
        card.EaseFactor.Should().Be(easeFactorBefore);
    }

    [Fact]
    public void Review_Easy_ShouldBoostIntervalAndIncreaseEaseFactor()
    {
        var card = CreateValidCard();
        var intervalBefore = card.Interval;
        var easeFactorBefore = card.EaseFactor;

        card.Review(QualityCard.Easy);

        card.Interval.Should().Be(Math.Max(1, (int)(intervalBefore * easeFactorBefore * 1.3)));
        card.Repetitions.Should().Be(1);
        card.EaseFactor.Should().Be(Math.Min(2.5, easeFactorBefore + 0.15));
    }

    [Fact]
    public void Review_Again_5Times_EaseFactorShouldNotGoBelowFloor()
    {
        var card = CreateValidCard();

        for (int i = 0; i < 5; i++)
            card.Review(QualityCard.Again);

        card.EaseFactor.Should().BeGreaterThanOrEqualTo(1.3);
    }

    [Fact]
    public void Review_Again_ManyTimes_EaseFactorShouldNotGoBelowFloor()
    {
        var card = CreateValidCard();

        for (int i = 0; i < 20; i++)
            card.Review(QualityCard.Again);

        card.EaseFactor.Should().Be(1.3);
    }

    [Fact]
    public void Review_Easy_5Times_EaseFactorShouldNotExceedCeiling()
    {
        var card = CreateValidCard();
        // Lower EaseFactor first so Easy has room to increase
        card.Review(QualityCard.Again);
        card.Review(QualityCard.Again);

        for (int i = 0; i < 5; i++)
            card.Review(QualityCard.Easy);

        card.EaseFactor.Should().BeLessThanOrEqualTo(2.5);
    }

    [Fact]
    public void Review_ShouldSetRecallDateToUtcNowPlusInterval()
    {
        var card = CreateValidCard();
        var before = DateTime.UtcNow;

        card.Review(QualityCard.Good);

        var after = DateTime.UtcNow;
        card.RecallDate.Should().BeOnOrAfter(before.AddDays(card.Interval - 1));
        card.RecallDate.Should().BeOnOrBefore(after.AddDays(card.Interval + 1));
    }

    [Theory]
    [InlineData(QualityCard.Again)]
    [InlineData(QualityCard.Hard)]
    [InlineData(QualityCard.Good)]
    [InlineData(QualityCard.Easy)]
    public void Review_ShouldRaiseCardReviewedEventWithCorrectQuality(string quality)
    {
        var card = CreateValidCard();

        card.Review(quality);

        card.DomainEvents.Should().ContainSingle(e => e is CardReviewedEvent)
            .Which.Should().BeOfType<CardReviewedEvent>()
            .Which.Quality.Should().Be(quality);
    }
}

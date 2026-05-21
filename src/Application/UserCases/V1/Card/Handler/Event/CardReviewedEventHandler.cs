using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using static Domain.Entities.Card;

namespace Application.UserCases.V1.Card.Handler.Event;

public class CardReviewedEventHandler : IDomainHandler<CardReviewedEvent>
{
    public Task Handle(CardReviewedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Card reviewed: {notification.Id} by owner {notification.OwnerId}");
        return Task.CompletedTask;
    }
}

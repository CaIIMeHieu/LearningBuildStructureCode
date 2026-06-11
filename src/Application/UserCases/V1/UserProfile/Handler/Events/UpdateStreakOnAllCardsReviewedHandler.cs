using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.UserCases.V1.UserProfile.Handler.Events;

public class UpdateStreakOnAllCardsReviewedHandler : IDomainHandler<Domain.Entities.UserProfile.AllCardsReviewedEvent>
{
    private readonly IRepositoryBase<Domain.Entities.UserProfile, Guid> _userProfileRepository;
    private readonly IRepositoryBase<Domain.Entities.Card, Guid> _cardRepository;

    public UpdateStreakOnAllCardsReviewedHandler(IRepositoryBase<Domain.Entities.UserProfile, Guid> userProfileRepository, IRepositoryBase<Domain.Entities.Card, Guid> cardRepository)
    {
        _userProfileRepository = userProfileRepository;
        _cardRepository = cardRepository;
    }

    public async Task Handle(Domain.Entities.UserProfile.AllCardsReviewedEvent notification, CancellationToken cancellationToken)
    {
        // Update user profile streak logic here
        // Query xem user có bị miss mất ngày nào không review không  
        var userProfile = await _userProfileRepository.FindByIdAsync(notification.UserId);        
        _ = userProfile ?? throw new InvalidOperationException($"UserProfile with ID {notification.UserId} not found.");

        bool hasMissedDays = false;
        var lastReviewedDate = userProfile.Streak.LastReviewedDate;
        if( lastReviewedDate.HasValue )
        {
            var fromDate = lastReviewedDate.Value.AddDays(1);
            var toDate = notification.ReviewDate.AddDays(-1);

            if( fromDate < toDate )
            {
                var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue);
                var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue);
                var missedCards = await _cardRepository.FindSingleAsync(c => c.OwnerId == notification.UserId
                    && fromDateTime <= c.RecallDate
                    && c.RecallDate <= toDateTime);

                hasMissedDays = missedCards is not null;
            }
        }
        userProfile.RecordReview(notification.ReviewDate, hasMissedDays);
        _userProfileRepository.Update(userProfile);
    }
}

// DateOnly khác gì DateTime

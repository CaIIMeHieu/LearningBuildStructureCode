using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions.Entities;
using Domain.Enumerations;

namespace Domain.Entities;

public class UserBadge : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public BadgeType BadgeType { get; private set; }
    public DateTime EarnedAt { get; private set; }

    protected UserBadge() { }

    private UserBadge(Guid id, Guid userId, BadgeType badgeType) : base(id)
    {
        UserId = userId;
        BadgeType = badgeType;
        EarnedAt = DateTime.UtcNow;
    }

    public static UserBadge Create(Guid userId, BadgeType badgeType)
        => new(Guid.NewGuid(), userId, badgeType);
}

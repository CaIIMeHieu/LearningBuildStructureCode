using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Application.UserCases.V1.UserProfile;

public class QuerySource
{
    public record GetMyProfileQuery(Guid UserId) : IQuery<Response.ProfileResponse>;
}

public class Response
{
    public record ProfileResponse(
        int StreakCurrent,
        int StreakLongest,
        DateOnly? LastReviewedDate,
        int TotalReviews,
        List<BadgeResponse> Badges);

    public record BadgeResponse(
        string Code,
        DateTime EarnedAt);
}

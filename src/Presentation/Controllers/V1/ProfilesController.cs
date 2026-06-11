using System.Security.Claims;
using Asp.Versioning;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;
using static Application.UserCases.V1.UserProfile.QuerySource;
using static Application.UserCases.V1.UserProfile.Response;

namespace Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class ProfilesController : ApiController
{
    public ProfilesController(ISender sender) : base(sender) { }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(Result<ProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await Sender.Send(new GetMyProfileQuery(userId));
        return result.IsSuccess ? Ok(result) : HandlerFailure(result);
    }
}

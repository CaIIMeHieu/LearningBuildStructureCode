using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;

namespace Presentation.Controllers.V1;

[ApiVersion(1)]
public class AuthsController: ApiController
{
    public AuthsController(ISender sender) : base(sender)
    {

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Application.UserCases.V1.Auth.CommandSource.LoginCommand loginCommand)
    {
        var result = await Sender.Send(loginCommand);
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Application.UserCases.V1.Auth.CommandSource.RegisterCommand registerCommand)
    {
        var result = await Sender.Send(registerCommand);
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }

    [HttpPost("renewAccesstoken")]
    public async Task<IActionResult> RenewAccessToken([FromBody] Application.UserCases.V1.Auth.CommandSource.RenewAccessTokenCommand command)
    {
        var result = await Sender.Send(command);
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] Application.UserCases.V1.Auth.CommandSource.RevolkRefreshTokenCommand command)
    {
        var result = await Sender.Send(command);
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }

    // Điều hướng đến popup login của google
    [HttpGet("login-google")]
    public IActionResult LoginGoogle([FromQuery] string deviceId)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback"),
            Items = { ["deviceId"] = deviceId } // framework tự encode vào state
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // Bước google đã trả về token cho clientid
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        var deviceId = result.Properties.Items["deviceId"]; // lấy từ state gửi lên và được nhận về

        if (!result.Succeeded)
            return Unauthorized();

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        //var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var command = new Application.UserCases.V1.Auth.CommandSource.GoogleLoginCommand(email, deviceId, googleId);
        var commandResult = await Sender.Send(command);

        if( commandResult.IsSuccess )
        {
            return Ok(commandResult);
        }
        return HandlerFailure(commandResult);
    }
}

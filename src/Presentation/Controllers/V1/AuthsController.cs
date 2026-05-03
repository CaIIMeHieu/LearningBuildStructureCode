using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
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
}

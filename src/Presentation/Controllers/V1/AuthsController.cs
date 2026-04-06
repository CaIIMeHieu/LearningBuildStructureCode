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
    public async Task<IActionResult> Login([FromBody] Application.UserCases.V1.Auth.Command.LoginCommand loginCommand)
    {
        var result = await Sender.Send(loginCommand);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Application.UserCases.V1.Auth.Command.RegisterCommand registerCommand)
    {
        var result = await Sender.Send(registerCommand);
        return Ok(result);
    }
}

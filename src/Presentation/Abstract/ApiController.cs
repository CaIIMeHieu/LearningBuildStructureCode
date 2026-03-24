using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Abstract;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;
    protected ApiController(ISender sender)
    {
        Sender = sender;
    }
}

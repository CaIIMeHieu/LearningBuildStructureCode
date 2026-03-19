using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(1)]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new[] { "Product 1", "Product 2", "Product 3" });
    }
}

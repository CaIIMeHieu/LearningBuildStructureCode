using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Attributes;
using Application.UserCases.V1.Product;
using Asp.Versioning;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;
using static Application.UserCases.V1.Product.Query;


namespace Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class ProductsController : ApiController
{
    public ProductsController(ISender sender) : base(sender)
    {

    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<List<Response.ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Cache(1000)]
    public async Task<IActionResult> GetProducts()
    {
        var result = await Sender.Send(new Query.GetProductsQuery());
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }


    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(Result<Response.ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Products( Guid productId )
    {
        var result = await Sender.Send( new GetProductByIdQuery(productId));
        if (result.IsSuccess)
        {
            return Ok(result); 
        }
        return HandlerFailure(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] Command.CreateProductCommand createProductCommand )
    { 
        var result = await Sender.Send(createProductCommand);
        return Ok(result);
    }

}

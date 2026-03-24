using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UserCases.V1.Product;
using Asp.Versioning;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;
using static Application.UserCases.V1.Product.Query;


namespace Presentation.Controllers.V1;

[ApiVersion(1)]
public class ProductsController : ApiController
{
    public ProductsController(ISender sender) : base(sender)
    {

    }

    [HttpGet("GetProducts")]
    [ProducesResponseType(typeof(Result<List<Response.ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts()
    {
        var result = await Sender.Send(new Query.GetProductsQuery());
        return Ok(result);
    }


    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(Result<Response.ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Products( Guid productId )
    {
        var result = await Sender.Send( new GetProductByIdQuery(productId));
        return Ok(result); 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UserCases.V1.Deck;
using Asp.Versioning;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;
using static Application.UserCases.V1.Deck.QuerySource;

namespace Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class DecksController : ApiController
{
    public DecksController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDeck([FromBody] CommandSource.CreateDeckCommand createDeckCommand)
    {
        var result = await Sender.Send(createDeckCommand);
        if( result.IsSuccess )
        {
            return Ok(result);
        }
        return HandlerFailure(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PageResultT<Domain.Entities.Deck>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // example request: GET /api/v1/decks?pageIndex=1&pageSize=10&sort=name-asc,description-desc
    public async Task<IActionResult> GetUserDecks([FromQuery] PagedRequest pagedRequest)
    {
        var result = await Sender.Send(new GetUserDecksQuery(pagedRequest));
        return result.IsSuccess ? Ok(result) : HandlerFailure(result);
    }
}

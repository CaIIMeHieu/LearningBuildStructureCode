using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.UserCases.V1.Card;
using Application.UserCases.V1.Card.Handler.Command;
using Asp.Versioning;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstract;
using static Application.UserCases.V1.Card.CommandSource;
using static Application.UserCases.V1.Card.QuerySource;
using static Application.UserCases.V1.Deck.QuerySource;

namespace Presentation.Controllers.V1;

[ApiVersion(1)]
[Authorize]
public class CardsController : ApiController
{
    public CardsController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{deckId}/due-cards")]
    [ProducesResponseType(typeof(Result<PageResultT<Response.CardResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDueCards([FromRoute] Guid DeckId, [FromQuery] PagedRequest pagedRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await Sender.Send(new GetDueCardsQuery(DeckId,userId,pagedRequest));
        return result.IsSuccess ? Ok(result) : HandlerFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardCommand createCardCommand)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await Sender.Send(new CreateCardCommand(createCardCommand.DeckId, createCardCommand.Question, createCardCommand.Answer, createCardCommand?.Note, userId));
        return result.IsSuccess ? Ok(result) :HandlerFailure(result);
    }

    [HttpPost("review-card")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReviewCard([FromBody] ReviewCardCommand reviewCardCommand)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await Sender.Send(new ReviewCardCommand(reviewCardCommand.CardId, reviewCardCommand.Quality, userId));
        return result.IsSuccess ? Ok(result) : HandlerFailure(result);
    }
}

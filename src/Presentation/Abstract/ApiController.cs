using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
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

    protected IActionResult HandlerFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            IValidationResult validationResult => BadRequest(
                    CreateProblemDetails(
                        "Validation Error", StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors)),
            { Error.ErrorType: ErrorTypeEnum.NotFound } => NotFound(CreateProblemDetails("Not Found", StatusCodes.Status404NotFound, result.Error)),
            { Error.ErrorType : ErrorTypeEnum.Conflict } => Conflict(CreateProblemDetails("Conflict", StatusCodes.Status409Conflict, result.Error)),
            { Error.ErrorType : ErrorTypeEnum.Unauthorized } => Unauthorized(CreateProblemDetails("Unauthorized", StatusCodes.Status401Unauthorized, result.Error)),
            _ => BadRequest(CreateProblemDetails("Bad Request", StatusCodes.Status400BadRequest, result.Error))
        };

    private static ProblemDetails CreateProblemDetails( string title, int status, Error error, Error[]? errors =null )
    {
        return new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = error.Message,
            Extensions =
            {{nameof(errors),errors}}
        };        
    }
}

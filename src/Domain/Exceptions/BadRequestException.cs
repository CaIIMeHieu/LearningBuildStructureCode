using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions;

public class BadRequestException : DomainException
{
    protected BadRequestException(string message)
        : base("Bad Request", message)
    {
    }
    public override int StatusCode { get; } = StatusCodes.Status400BadRequest;
}

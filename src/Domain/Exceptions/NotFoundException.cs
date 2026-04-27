using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base("Not Found", message)
    {
    }
    public override int StatusCode => StatusCodes.Status404NotFound;
}

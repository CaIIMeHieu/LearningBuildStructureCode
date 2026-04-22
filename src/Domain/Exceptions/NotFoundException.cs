using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions;

public class NotFoundException : DomainException
{
    protected NotFoundException(string message)
        : base("Not Found", message)
    {
    }
    public override int StatusCode => StatusCodes.Status404NotFound;
}

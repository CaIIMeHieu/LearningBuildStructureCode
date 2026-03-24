using Contract.Abstractions.Message;

namespace Application.UserCases.V1.Product;

public static class Command
{
    public record CreateProductCommand(string Name, decimal Price, string Description) : ICommand;
}

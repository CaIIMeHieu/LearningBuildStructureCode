using Contract.Abstractions.Shared;
using FluentAssertions;

namespace Test.Unit;

public class ErrorTests
{
    [Fact]
    public void NotFound_ShouldCreate_WithCorrectProperties()
    {
        // Arrange
        var code = "Product.NotFound";
        var message = "Product not found";

        // Act
        var error = Error.NotFound(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
    }

    [Fact]
    public void Validation_ShouldCreate_WithCorrectProperties()
    {
        // Arrange
        var code = "Product.Validation";
        var message = "Product validation failed";
        // Act
        var error = Error.Validation(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.ErrorType.Should().Be(ErrorTypeEnum.Validation);
    }

    [Fact]
    public void Conflict_ShouldCreate_WithCorrectProperties()
    {
        // Arrange
        var code = "Product.Conflict";
        var message = "Product conflict error";
        // Act
        var error = Error.Conflict(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.ErrorType.Should().Be(ErrorTypeEnum.Conflict);
    }
}

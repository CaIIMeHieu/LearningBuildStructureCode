using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using FluentAssertions;

namespace Test.Unit;

public class DeckTests
{
    [Fact]
    public void ValidCreateDeck()
    {
        // Chuẩn bị 
        var name = "ASP.NET CORE";
        var description = "This is a deck about ASP.NET CORE";
        var ownerId = Guid.NewGuid();
        
        // Tiến hành
        var deck = Deck.Create(name, description, ownerId);

        // Kiểm tra
        Assert.Equal(name,deck.Name.Value);
        Assert.Equal(description, deck.Description);
        Assert.Equal(ownerId, deck.OwnerId);
        Assert.NotEqual(Guid.Empty, deck.Id);
    }

    [Fact]
    public void CreateDeck_WithEmptyName_ShouldThrowException()
    {
        // Chuẩn bị 
        var name = "";
        var description = "This is a deck about ASP.NET CORE";
        var ownerId = Guid.NewGuid();
        
        // Tiến hành & Kiểm tra
        Assert.Throws<ArgumentException>(() => Deck.Create(name, description, ownerId));
    }
    
    [Fact]
    public void CreateDeck_WithTooLongName_ShouldThrowException()
    {
        // Chuẩn bị 
        var name = new string('A', 101); // Tạo chuỗi dài 101 ký tự
        var description = "This is a deck about ASP.NET CORE";
        var ownerId = Guid.NewGuid();
        
        // Tiến hành & Kiểm tra
        Assert.Throws<ArgumentException>(() => Deck.Create(name, description, ownerId));
    }
    
    [Fact]
    public void CreateDeck_WithNullDescription()
    {
        // Chuẩn bị 
        var name = "ASP.NET CORE";
        string description = null;
        var ownerId = Guid.NewGuid();
        
        // Tiến hành & Kiểm tra
        Deck.Create(name, description, ownerId);
        Assert.Null(description);
    }

    [Fact]
    public void CreateDeck_WithHaveToDeckCreatedEvent()
    {
        // Chuẩn bị 
        var name = "ASP.NET CORE";
        var description = "new string('A', 501); // Tạo chuỗi dài 501 ký tự";
        var ownerId = Guid.NewGuid();
        
        // Tiến hành & Kiểm tra
        var deck = Deck.Create(name, description, ownerId);

        deck.DomainEvents.Should().ContainSingle(e => e is DeckCreatedEvent)
            .Which.Should().BeOfType<DeckCreatedEvent>()
            .Which.Name.Should().Be(name);
    }
}

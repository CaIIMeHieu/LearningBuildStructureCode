using Contract.Abstractions.Message;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Deck : AggregateRoot
{
    public DeckName Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    protected Deck() { }

    private Deck(Guid id, string name, string? description, Guid ownerId): base(id)
    {
        Name = new DeckName(name);
        Description = description;
        OwnerId = ownerId;
    }

    public static Deck Create( string name, string? description, Guid ownerId )
    {
        // description is optional, tối đa 500 ký tự
        if( string.IsNullOrWhiteSpace(description) )
        {
            description = null;
        }
        else if ( !string.IsNullOrEmpty(description) && description.Length > 500)
            throw new ArgumentException("Deck description cannot exceed 500 characters.");
        if ( ownerId == Guid.Empty )
            throw new ArgumentException("Deck owner ID cannot be empty.");
        var deck = new Deck(Guid.NewGuid(), name, description, ownerId);
        deck.RaiseDomainEvent(new DeckCreatedEvent(deck.Id, deck.Name.Value, deck.Description, deck.OwnerId));
        return deck;
    }

}

public class DeckName
{
    public string Value { get; private set; }
    protected DeckName() { }
    public DeckName( string value )
    {
        if( string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Deck name cannot be empty or null");
        }
        else if( value.Length > 100 )
        {
            throw new ArgumentException("Deck name cannot exceed 100 characters.");
        }    
        Value = value.Trim();
    }  
}

// event không nên expose value object, vì có thể có logic validate trong đó, nên chỉ expose giá trị đơn giản
public record DeckCreatedEvent(Guid Id, string Name, string? Description, Guid OwnerId) : IDomain;

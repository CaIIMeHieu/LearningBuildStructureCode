using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }

    public static Product Create( string Name, decimal Price, string Description )
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Price = Price,
            Description = Description
        };

        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Price, product.Description));

        return product;
    }

    public void Update( string Name, decimal Price, string Description )
    {
        this.Name = Name;
        this.Price = Price;
        this.Description = Description;
        // Có thể Raise một sự kiện khác nếu cần
    }

    public record ProductCreatedEvent(Guid Id, string Name, decimal Price, string Description) : IDomain;

}

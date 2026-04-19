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

    protected Product() { }
    private Product(Guid id) : base(id)
    {
    }

    public static Product Create( string name, decimal price, string description )
    {
        var product = new Product(Guid.NewGuid())
        {
            Name = name,
            Price = price,
            Description = description
        };

        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Price, product.Description));

        return product;
    }

    public void Update( string name, decimal price, string description )
    {
        Name = name;
        Price = price;
        Description = description;
        // Có thể Raise một sự kiện khác nếu cần
    }

    public record ProductCreatedEvent(Guid Id, string Name, decimal Price, string Description) : IDomain;

    /*
      Cấu trúc ngắn gọn, tương đương với 
        public class ProductCreatedEvent
        {
            public Guid Id { get; init; }
            public string Name { get; init; }
            public decimal Price { get; init; }
            public string Description { get; init; }

            public ProductCreatedEvent(Guid Id, string Name, decimal Price, string Description)
            {
                this.Id = Id;
                this.Name = Name;
                this.Price = Price;
                this.Description = Description;
            }
        }
    */

}

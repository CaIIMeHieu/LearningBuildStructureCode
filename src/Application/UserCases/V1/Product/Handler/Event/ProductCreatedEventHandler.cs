using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Application.UserCases.V1.Product.Handler.Event;

public class ProductCreatedEventHandler : IDomainHandler<Domain.Entities.Product.ProductCreatedEvent>
{
    public Task Handle(Domain.Entities.Product.ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Product created: " + notification.Id);
        return Task.CompletedTask;
    }
}

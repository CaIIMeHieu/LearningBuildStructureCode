using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Domain.Abstractions.Entities;

public class AggregateRoot : DomainEntity<Guid>
{
    private readonly List<IDomain> _domainEvents = new List<IDomain>();

    public IReadOnlyList<IDomain> DomainEvents => _domainEvents.AsReadOnly();
    protected AggregateRoot() : base()
    {
    }
    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected void RaiseDomainEvent(IDomain domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }

}

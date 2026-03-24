using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities;

public abstract class DomainEntity<TKey>
{
    public virtual TKey Id { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}

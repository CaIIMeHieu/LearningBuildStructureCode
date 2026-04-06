using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DependencyInjection.Options;

public class RedisOptions
{
    public bool Enable { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}

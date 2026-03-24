using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Abstractions.Shared;

public interface IValidationResult
{
    static readonly Error ValidationError = new(
        "ValidationError",
        "A validation problem occurred.");
    Error[] Errors { get; }
}

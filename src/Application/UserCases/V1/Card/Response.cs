using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.V1.Card;

public class Response
{
    public record CardResponse(
        Guid Id,
        string Name,
        string Description,
        DateTime DueDate
    );
}

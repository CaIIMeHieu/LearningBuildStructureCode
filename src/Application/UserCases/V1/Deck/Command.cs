using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Application.UserCases.V1.Deck;

public class CommandSource
{
    public record CreateDeckCommand(string Name, string? Description) : ICommand;
}

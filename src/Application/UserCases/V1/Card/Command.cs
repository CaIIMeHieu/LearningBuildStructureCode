using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;

namespace Application.UserCases.V1.Card;

public class CommandSource
{
    public record CreateCardCommand(Guid DeckId, string Question, string Answer, string? Note) : ICommand;
    public record ReviewCardCommand(Guid CardId, string Quality) : ICommand;
}

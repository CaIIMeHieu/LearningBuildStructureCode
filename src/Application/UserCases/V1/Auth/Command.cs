using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Domain.Entities;
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth;

public class Command
{
    public record LoginCommand(string Email, string Password)
    : ICommand<LoginResponse>;

    public record RegisterCommand(string FullName, string Email, string Password) : ICommand<LoginResponse>;
}

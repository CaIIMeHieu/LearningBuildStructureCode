using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Domain.Entities;
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth;

public class CommandSource
{
    public record LoginCommand(string Email, string Password, string DeviceId)
    : ICommand<LoginResponse>;

    public record RegisterCommand(string FullName, string Email, string Password, string DeviceId) : ICommand<LoginResponse>;
    public record RevolkRefreshTokenCommand(Guid userId, string refreshToken, string deviceId) : ICommand<bool>;
    public record RenewAccessTokenCommand(string refreshToken, string deviceId) : ICommand<LoginResponse>;
}

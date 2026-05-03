using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using static Application.UserCases.V1.Auth.CommandSource;
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth.Handler;

public class LoginHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IRepositoryBase<RefreshToken, Guid> _refreshTokenRepository; 

    public LoginHandler(UserManager<AppUser> userManager, IJwtTokenService jwtService, IRepositoryBase<RefreshToken, Guid> refreshTokenRepository )
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command, CancellationToken ct)
    {
        // 1. Tìm user
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Result.Failure<LoginResponse>(
                new Error("Auth.Login", "Invalid email or password."));

        // 2. Verify password
        var isValid = await _userManager.CheckPasswordAsync(user, command.Password);
        if (!isValid)
            return Result.Failure<LoginResponse>(
                new Error("Auth.Login", "Invalid email or password."));

        var (accessToken, refreshTokenString) = await _jwtService.IssueTokenAsync(user, command.DeviceId, ct);
 
        return Result.Success(new LoginResponse(accessToken, refreshTokenString));
    }

    public bool IsExpireTime( DateTime expiryTime)
    {
        return DateTime.UtcNow > expiryTime;
    }
}

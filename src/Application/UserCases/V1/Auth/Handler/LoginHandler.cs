using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using static Application.UserCases.V1.Auth.Command;
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth.Handler;

public class LoginHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtService;

    public LoginHandler(UserManager<AppUser> userManager, IJwtTokenService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
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

        // 3. Lấy roles
        var roles = await _userManager.GetRolesAsync(user);

        // 4. Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 5. Lưu Refresh Token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow
            .AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new LoginResponse(accessToken, refreshToken));
    }
}

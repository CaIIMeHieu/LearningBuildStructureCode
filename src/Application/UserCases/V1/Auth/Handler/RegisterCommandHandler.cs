using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Application.UserCases.V1.Auth.Handler;

public class RegisterCommandHandler : ICommandHandler<Command.RegisterCommand, Response.LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtService;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtService = jwtTokenService;
    }
    public async Task<Result<Response.LoginResponse>> Handle(Command.RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email, // Identity dùng UserName để login
            PasswordHash = request.Password // Password sẽ được hash tự động bởi UserManager.CreateAsync
        };        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description));
            return Result.Failure<Response.LoginResponse>(errors.First());
        }

        List<string> roles = new List<string> { "User" };
        await _userManager.AddToRolesAsync(user, roles);

        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new Response.LoginResponse(accessToken,refreshToken));

    }
}

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
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth.Handler.Command;

public class GoogleLoginCommandHandler : ICommandHandler<CommandSource.GoogleLoginCommand, LoginResponse>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<AppUser> _userManager;

    public GoogleLoginCommandHandler( IJwtTokenService jwtTokenService, UserManager<AppUser> userManager)
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }
    public async Task<Result<LoginResponse>> Handle(CommandSource.GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);
        if( user == null )
        {
            // trường hợp đăng ký
            // Tạo user mới, không có password
            user = new AppUser { Email = request.email, UserName = request.email };
            await _userManager.CreateAsync(user); // không truyền password

            // Link Google account vào user này
            await _userManager.AddLoginAsync(user, new UserLoginInfo(
                loginProvider: "Google",
                providerKey: request.googleId,      // sub từ Google
                displayName: "Google"
            ));
        }
        var (accessToken, refreshToken) = await _jwtTokenService.IssueTokenAsync(user, request.deviceId, cancellationToken);
        return Result.Success<LoginResponse>(new LoginResponse(accessToken, refreshToken));
    }
}

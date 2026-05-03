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
using static Application.UserCases.V1.Auth.Response;

namespace Application.UserCases.V1.Auth.Handler.Command;

public class RenewAccessTokenCommandHandler : ICommandHandler<CommandSource.RenewAccessTokenCommand, LoginResponse>
{
    private readonly IRepositoryBase<RefreshToken, Guid> _refreshTokenRepository;
    private readonly IJwtTokenService _jwtService;
    private readonly UserManager<AppUser> _userManager;
    public RenewAccessTokenCommandHandler(IRepositoryBase<RefreshToken, Guid> refreshTokenRepository, IJwtTokenService jwtService, UserManager<AppUser> userManager)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _userManager = userManager;
    }
    public async Task<Result<LoginResponse>> Handle(CommandSource.RenewAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.FindSingleAsync(refreshToken => refreshToken.DeviceId == request.deviceId && refreshToken.Token.Equals(request.refreshToken) && DateTime.UtcNow < refreshToken.ExpireAt);
        if( refreshToken == null )
        {
            return Result.Failure<LoginResponse>( Error.NotFound("RefreshToken Error","Refresh token not found or invalid force logout user"));
        }
        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshTokenString = RefreshToken.GenerateRefreshToken();
        refreshToken.Update(refreshTokenString,DateTime.UtcNow.AddDays(7));
        _refreshTokenRepository.Update(refreshToken);
        return Result.Success(new LoginResponse(accessToken,refreshToken.Token));
    }
}

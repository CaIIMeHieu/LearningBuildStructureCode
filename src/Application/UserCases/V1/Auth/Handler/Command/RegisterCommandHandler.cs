using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Application.UserCases.V1.Auth.Handler.Command;

public class RegisterCommandHandler : ICommandHandler<CommandSource.RegisterCommand, Response.LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IRepositoryBase<RefreshToken, Guid> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService, IRepositoryBase<RefreshToken, Guid> refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Response.LoginResponse>> Handle(CommandSource.RegisterCommand request, CancellationToken cancellationToken)
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
        var refreshTokenString = RefreshToken.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, request.DeviceId, refreshTokenString, DateTime.UtcNow.AddDays(7));
        _refreshTokenRepository.Add(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new Response.LoginResponse(accessToken,refreshTokenString));

    }
}

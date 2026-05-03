using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepositoryBase<RefreshToken, Guid> _refreshTokenRepository;

    public JwtTokenService(IOptions<JwtSettings> options, UserManager<AppUser> userManager, IRepositoryBase<RefreshToken, Guid> refreshTokenRepository)
        => (_settings, _userManager, _refreshTokenRepository) = (options.Value, userManager, refreshTokenRepository);

    public async Task<(string accessToken, string refreshToken)> IssueTokenAsync( AppUser user, string deviceId, CancellationToken ct )
    {
        // 1. Lấy roles
        var roles = await _userManager.GetRolesAsync(user);

        // 4. Generate tokens
        var accessToken = GenerateAccessToken(user, roles);
        var exitsValidRefreshToken = await _refreshTokenRepository.FindSingleAsync(token => token.UserId == user.Id && token.DeviceId == deviceId && token.ExpireAt > DateTime.UtcNow);
        var refreshTokenString = "";
        // 5. Lưu Refresh Token
        if (exitsValidRefreshToken != null)
        {
            refreshTokenString = exitsValidRefreshToken.Token;
        }
        else
        {
            refreshTokenString = RefreshToken.GenerateRefreshToken();
            var refreshToken = RefreshToken.Create(user.Id, deviceId, refreshTokenString, DateTime.UtcNow.AddDays(7));
            _refreshTokenRepository.Add(refreshToken);
        }
        return (accessToken, refreshTokenString);
    }
    public string GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Thêm roles vào claims
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

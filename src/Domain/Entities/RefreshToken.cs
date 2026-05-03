using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class RefreshToken : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpireAt { get; private set; }
    public DateTime CreateAt { get; private set; }

    protected RefreshToken() { }

    private RefreshToken( Guid id ) : base(id)
    {

    }

    public static RefreshToken Create( Guid userId, string deviceId, string token, DateTime expireAt )
    {
        return new RefreshToken(Guid.NewGuid())
        {
            UserId = userId,
            DeviceId = deviceId,
            Token = token,
            ExpireAt = expireAt,
            CreateAt = DateTime.UtcNow
        };
    }

    public void Update( string refreshToken,DateTime expireAt)
    {
        Token = refreshToken;
        ExpireAt = expireAt;
        CreateAt = DateTime.UtcNow;
    }

    public static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

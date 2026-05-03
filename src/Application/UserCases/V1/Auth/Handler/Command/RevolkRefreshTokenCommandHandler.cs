using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.Abstractions.Message;
using Contract.Abstractions.Shared;
using Domain.Abstractions;

namespace Application.UserCases.V1.Auth.Handler.Command;

public class RevolkRefreshTokenCommandHandler : ICommandHandler<CommandSource.RevolkRefreshTokenCommand, bool>
{
    private readonly IRepositoryBase<Domain.Entities.RefreshToken, Guid> _refreshTokenRepository;
    public RevolkRefreshTokenCommandHandler(IRepositoryBase<Domain.Entities.RefreshToken, Guid> refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }
    public async Task<Result<bool>> Handle(CommandSource.RevolkRefreshTokenCommand request, CancellationToken cancellationToken)
    {
       var refreshToken = await _refreshTokenRepository.FindSingleAsync( refreshToken => refreshToken.UserId == request.userId && refreshToken.DeviceId == request.deviceId, cancellationToken); 
       if( refreshToken == null )
       {
           return Result.Failure<bool>( Error.NotFound("RefreshToken Error","Refresh token not found or invalid"));
       }
       _refreshTokenRepository.Remove(refreshToken);
       return Result.Success(true);
    }
}

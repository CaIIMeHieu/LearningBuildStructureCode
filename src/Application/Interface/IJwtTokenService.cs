using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Authentication;

public interface IJwtTokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.V1.Auth;

public class Response
{
    public record LoginResponse(string AccessToken, string RefreshToken);
}

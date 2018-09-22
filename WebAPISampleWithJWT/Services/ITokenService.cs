using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;

namespace WebAPISampleWithJWT.Services
{
    public interface ITokenService
    {
        string generateAccessToken(IEnumerable<Claim> claims);
        string generateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredExpiredToken(string token);
        IEnumerable<User> GetAll();
    }
}

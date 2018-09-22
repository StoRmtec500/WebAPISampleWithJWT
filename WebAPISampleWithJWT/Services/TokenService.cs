using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;

namespace WebAPISampleWithJWT.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserContext _userContext;
        private readonly IConfiguration _config;
        public string tokengen { get; set; }

        public TokenService(IConfiguration config, UserContext userContext)
        {
            _config = config;
            _userContext = userContext;
        }

        public string generateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _refresh_token = Guid.NewGuid().ToString().Replace("-", "");

            var SecurityToken = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: creds);

            var tok = SecurityToken;

            return tok.ToString();

        }

        public string generateRefreshToken()
        {
            var randomNumber = Guid.NewGuid().ToString();
            return randomNumber;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            try { 
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
            }
            catch
            {
                throw new SecurityTokenValidationException();
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _userContext.User;
        }
    }
}

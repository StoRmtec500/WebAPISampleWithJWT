using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;
using WebAPISampleWithJWT.Resource;
using WebAPISampleWithJWT.Services;

namespace WebAPISampleWithJWT.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly UserContext _userContext;
        private readonly IConfiguration _config;

        public TokenController(ITokenService tokenService, UserContext userContext, IConfiguration config)
        {
            _tokenService = tokenService;
            _userContext = userContext;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenResource tokenResource)
        {
            var principal = _tokenService.GetPrincipalFromExpiredExpiredToken(tokenResource.Token);
            var username = principal.Identity.Name;

            var user = _userContext.User.SingleOrDefault(u => u.username == username);
            if (user == null || user.RefreshToken != tokenResource.refreshToken) return BadRequest();

            var newRefreshToken = _tokenService.generateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userContext.SaveChangesAsync();

           // var newJwtToken = _tokenService.generateAccessToken(principal.Claims);
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _refresh_token = Guid.NewGuid().ToString().Replace("-", "");

            var SecurityToken = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
            claims: principal.Claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(SecurityToken),
                notBefore = SecurityToken.ValidFrom,
                token_expiration = SecurityToken.ValidTo,
                refresh_token = newRefreshToken,
                IssuedUtc = DateTime.UtcNow,
                refresh_token_expire = DateTime.UtcNow.AddDays(20)
            });

        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetAll()
        {
            var mitarbeiter = _tokenService.GetAll();
            return Ok(mitarbeiter);
        }
    }
}

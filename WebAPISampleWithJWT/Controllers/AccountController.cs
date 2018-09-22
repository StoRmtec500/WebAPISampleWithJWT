using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPISampleWithJWT.Models;
using WebAPISampleWithJWT.Services;

namespace WebAPISampleWithJWT.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly UserContext _userContext;

        public AccountController(UserContext userContext, ITokenService tokenService)
        {
            _userContext = userContext;
            _tokenService = tokenService;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody]User users)
        {
            var user = _userContext.User.SingleOrDefault(u => u.username == users.username);
            if (user != null) return Conflict(new { message = "Username ist schon vorhanden!" }); ;
            _userContext.User.Add(new User
            {
                username = users.username,
                password = users.password
            });

            await _userContext.SaveChangesAsync();
            return Ok(new { message = "Username und Passwort wurden gespeichert!", user});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]User users)
        {
            var user = _userContext.User.SingleOrDefault(u => u.username == users.username);
            if (user == null || user.password != users.password) return BadRequest();

            var usersClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var jwtToken = _tokenService.generateAccessToken(usersClaims);
            var refreshToken = _tokenService.generateRefreshToken();

            user.RefreshToken = refreshToken;
            await _userContext.SaveChangesAsync();

            return new ObjectResult(new
            {
                token = jwtToken,
                refreshToken = refreshToken
            });

        }
    }
}

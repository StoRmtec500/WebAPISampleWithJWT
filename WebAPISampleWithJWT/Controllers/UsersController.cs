using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPISampleWithJWT.Models;
using WebAPISampleWithJWT.Services;

namespace WebAPISampleWithJWT.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IMitarbeiterService _mitarbeiterService;
        private IConfiguration _config;

        public UsersController(IMitarbeiterService mitarbeiterService, IConfiguration config)
        {
            _mitarbeiterService = mitarbeiterService;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Mitarbeiter mitarbeiter)
        {

            if (mitarbeiter.Name != null && !string.IsNullOrWhiteSpace(mitarbeiter.Name))
            {
                if (mitarbeiter.LoginErlaubt == 1)
                {
                    var user = _mitarbeiterService.Authenticate(mitarbeiter.Name, mitarbeiter.Passwort);
                    if (user != null ){
                        {
                            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var _refresh_token = Guid.NewGuid().ToString().Replace("-", "");

                            var claims = new[]
                                    {
                          new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };


                            var SecurityToken = new JwtSecurityToken(
                            issuer: _config["Jwt:Issuer"],
                            audience: _config["Jwt:Issuer"],
                            claims: claims,
                            notBefore: DateTime.UtcNow,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: creds);

                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(SecurityToken),
                                notBefore = SecurityToken.ValidFrom,
                                token_expiration = SecurityToken.ValidTo,
                                refresh_token = _refresh_token,
                                refresh_token_expire = DateTime.UtcNow.AddDays(20)
                            });
                        }
                    }
                }
                return BadRequest(new { message = "Username or password is incorrect!" });
            }
            return BadRequest(new { message = "Username oder Passwort wurde nicht übermittelt" });
            
        }


        [AllowAnonymous]
        [HttpPost("refreshToken/{token}")]
        public void RefreshToken(string token)
        {
            var refreshToken = token.ToString();

            if (string.IsNullOrEmpty(refreshToken))
            {
                Response.StatusCode = 400;
                Response.WriteAsync("User must relogin.");
                return;
            }

            return ;
        }


        // GET: api/Users
        [HttpGet]
        public IActionResult GetAll()
        {
            var mitarbeiter = _mitarbeiterService.GetAll();
            return Ok(mitarbeiter);
        }
    }
}
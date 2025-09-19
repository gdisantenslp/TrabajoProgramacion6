﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrabajoProyecto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrabajoProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _config;
        public AuthController (IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody]LoginRequest req)
        {
            var validUser = _config["DemoUser:Username"];
            var validPass = _config["DemoUser:Password"];

            if (req.Username != validUser ||  req.Password != validPass)
            {
                return Unauthorized(new {message = "Credencials incorrectas. "});
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, req.Username),
                new Claim(ClaimTypes.Role, "Admin"),
              
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials:creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return Ok(new LoginResponse { Token = jwt, ExpiresAtUtc = token.ValidTo});
        }
    }
}

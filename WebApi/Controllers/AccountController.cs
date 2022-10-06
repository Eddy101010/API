using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration configuration;

        public AccountController(IUnitOfWork uow, IConfiguration configuration)
        {
            this.uow = uow;
            this.configuration = configuration;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginReqDto loginReq)    
        {
            var user = await uow.UserRepository.Authenticate(loginReq.UserName, loginReq.Password);

            if (user == null)
            {
                return Unauthorized("Invalid User ID or Password");  
            }

            var loginResponse = new LoginResponseDto();
            loginResponse.UserName = user.Username;
            loginResponse.Token = CreateJWT(user);
            return Ok(loginResponse);    
        }


        [HttpPost("register")]

        public async Task<IActionResult> Register(LoginReqDto loginReq)
        {
            if (await uow.UserRepository.UserAlreadyExists(loginReq.UserName))
                return BadRequest("User already exists");

            uow.UserRepository.Register(loginReq.UserName, loginReq.Password);
            await uow.SaveAsync();
            return StatusCode(201);
        }

            private string CreateJWT(User user)
        {
            var secretKey = configuration.GetSection("AppSettings:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}

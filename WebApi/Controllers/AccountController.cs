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

        public AccountController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginReqDto loginReq)    
        {
            var user = await uow.UserRepository.Authenticate(loginReq.UserName, loginReq.Password);

            if (user == null)
            {
                return Unauthorized();  
            }

            var loginResponse = new LoginResponseDto();
            loginResponse.UserName = user.Username;
            loginResponse.Token = CreateJWT(user);
            return Ok(loginResponse);    
        }

        private string CreateJWT(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("shhh.. this is my top secret")); 
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnboardingService.Models;

namespace OnboardingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardController : ControllerBase
    {
        
        private UserContext context;

        public OnboardController(UserContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel Model)
        {
            var user = await context.User.Where(existUser => 
            existUser.Username == Model.Username
            && existUser.Password==Model.Password
            && existUser.WorkspaceName==Model.WorkspaceName)
            .FirstOrDefaultAsync();

            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email,user.Username),
                };

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureKey"));
                var token = new JwtSecurityToken(
                    issuer: "http://oec.com",
                    audience: "http://oec.com",
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: claims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256));

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.User.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
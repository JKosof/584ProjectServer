using _584ProjectServer.Server.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SchoolModel;
using System.IdentityModel.Tokens.Jwt;

namespace _584ProjectServer.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<SchoolModelUser> userManager, JwtHandler jwtHandler) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            SchoolModelUser? user = await userManager.FindByNameAsync(loginRequest.UserName);
            if (user is null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized("Invalid username or password.");
            }

            JwtSecurityToken token = await jwtHandler.GenerateTokenAsync(user);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponse { 
                Success = true,
                Message = "Login successful.",
                Token = tokenString
            });
        }
    }
}
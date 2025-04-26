using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserTokenActivitiesRepository _tokenRepo;

        public AuthController(IUserTokenActivitiesRepository tokenRepo)
        {
            _tokenRepo = tokenRepo;
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken([FromQuery] string token)
        {
            try
            {
                await _tokenRepo.DisableTokenInvalidTokenUse(token);
                return Ok(new { IsValid = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { IsValid = false, Message = ex.Message });
            }
            catch
            {
                return Unauthorized(new { IsValid = false, Message = "Token validation failed" });
            }
        }
    }
}

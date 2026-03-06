using IceCity.Application.Dtos;
using IceCity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceCity.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            if(!string.IsNullOrEmpty(result.RefreshToken))
            {
                 SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);
        }

         [HttpPost("login")]
         public async Task<IActionResult> Login(LoginDto dto)
         {
            var result = await _authService.LoginAsync(dto);

            if (!result.IsAuthenticated)
            {
                 return Unauthorized(result.Message);
            }
    
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                 SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
    
             return Ok(result);
           
         }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto? dto)
        {
            dto.RefreshToken = dto.RefreshToken ?? Request.Cookies["refreshToken"];

            dto.AccessToken = dto.AccessToken ?? Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(dto.RefreshToken) || string.IsNullOrEmpty(dto.AccessToken))
            {
                return BadRequest("Tokens are required");
            }

            var result = await _authService.RefreshTokenAsync(dto);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody]string? refreshToken)
        {
            var token = refreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var result = await _authService.RevokeRefreshTokenAsync(token);

            if (!result)
            {
                return BadRequest("Token revocation failed");
            }


            return Ok("Token revoked successfully");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string? refreshToken)
        {
            var token = refreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var result = await _authService.RevokeRefreshTokenAsync(token);

            if (!result)
            {
                return BadRequest("Logout failed");
            }

            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out successfully");
        }
        private void SetRefreshTokenInCookie(string refreshToken,DateTime ExpiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = ExpiresOn,
                Secure = true, 
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

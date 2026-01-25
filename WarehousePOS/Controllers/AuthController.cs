using Microsoft.AspNetCore.Mvc;
using WarehousePOS.DTOs;
using WarehousePOS.Services;

namespace WarehousePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Step 1: Generate App Token dari External API (WIT)
        /// POST /api/Auth/generate-token
        /// </summary>
        [HttpPost("generate-token")]
        public async Task<ActionResult<GenerateTokenResponse>> GenerateToken([FromBody] GenerateTokenRequest? request = null)
        {
            var result = await _authService.GenerateTokenAsync(request);
            
            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Step 2: Login dengan credentials (akan otomatis generate token dulu)
        /// POST /api/Auth/login
        /// Body: { "identifier": "test@wit.id", "password": "123456" }
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Identifier) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Identifier (email) and password are required"
                });
            }

            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Full Authentication: Generate Token + Login dalam satu request
        /// POST /api/Auth/full-auth
        /// </summary>
        [HttpPost("full-auth")]
        public async Task<ActionResult<AuthResponse>> FullAuth([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Identifier) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Identifier (email) and password are required"
                });
            }

            var result = await _authService.FullAuthAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}

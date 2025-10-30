using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Core.DTOs.Request;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestDTO requestDTO)
        {
            var response = await _authService.Accesstoken(requestDTO);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Endpoint de prueba. Solo accesible con un token valido
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "¡Acceso concedido! Este es un endpoint protegido." });
        }

    }
}

// AuthController.cs
using ClinAgenda.src.Application.AuthUseCase;
using ClinAgenda.src.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCase _authUseCase;

        public AuthController(AuthUseCase authUseCase)
        {
            _authUseCase = authUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authUseCase.LoginAsync(loginDto);
            if (response == null)
                return Unauthorized("Nome de usuário ou senha inválidos");

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authUseCase.RegisterAsync(registrationDto);
                if (response == null)
                    return StatusCode(500, "Erro ao registrar usuário");

                return CreatedAtAction(nameof(Login), response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return BadRequest("Token inválido");

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var success = await _authUseCase.LogoutAsync(token);

            if (!success)
                return BadRequest("Falha ao encerrar a sessão");

            return Ok(new { message = "Sessão encerrada com sucesso" });
        }

        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token não fornecido");

            var isValid = await _authUseCase.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.DTOs.Auth;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Autentica um usuário e retorna o token JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest(new { erro = "Login e senha são obrigatórios." });

        var resposta = await _auth.AutenticarAsync(request, ct);
        if (resposta is null)
            return Unauthorized(new { erro = "Credenciais inválidas." });

        return Ok(resposta);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Usuarios;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers;

/// <summary>Autoatendimento do usuário autenticado: ver/editar os próprios dados e trocar a senha.</summary>
[ApiController]
[Route("api/conta")]
[Authorize]
public class ContaController : ControllerBase
{
    private readonly IUsuarioService _service;

    public ContaController(IUsuarioService service)
    {
        _service = service;
    }

    /// <summary>Dados do usuário autenticado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> Minha(CancellationToken ct)
    {
        if (User.ObterUsuarioId() is not { } id) return Unauthorized();
        return Ok(await _service.ObterPorIdAsync(id, ct));
    }

    /// <summary>Atualiza os próprios dados (nome e e-mail).</summary>
    [HttpPut]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Atualizar([FromBody] AtualizarMinhaContaRequest request, CancellationToken ct)
    {
        if (User.ObterUsuarioId() is not { } id) return Unauthorized();
        return Ok(await _service.AtualizarMinhaContaAsync(id, request, ct));
    }

    /// <summary>Troca a própria senha (exige a senha atual).</summary>
    [HttpPut("senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarMinhaSenhaRequest request, CancellationToken ct)
    {
        if (User.ObterUsuarioId() is not { } id) return Unauthorized();
        await _service.AlterarMinhaSenhaAsync(id, request, ct);
        return NoContent();
    }
}

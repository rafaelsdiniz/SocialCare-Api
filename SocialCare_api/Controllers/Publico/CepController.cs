using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.DTOs.Publico;
using SocialCare.Application.Interfaces;

namespace SocialCare.Controllers.Publico;

[ApiController]
[Route("api/publico/cep")]
[AllowAnonymous]
public class CepController : ControllerBase
{
    private readonly IViaCepClient _viaCep;

    public CepController(IViaCepClient viaCep)
    {
        _viaCep = viaCep;
    }

    /// <summary>Proxy ViaCEP: retorna endereço a partir do CEP.</summary>
    [HttpGet("{cep}")]
    [ProducesResponseType(typeof(CepResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CepResponse>> Consultar(string cep, CancellationToken ct)
    {
        var resposta = await _viaCep.ConsultarAsync(cep, ct);
        if (resposta is null)
            return NotFound(new { erro = $"CEP '{cep}' não encontrado ou inválido." });

        return Ok(resposta);
    }
}

using System.Net.Http.Json;
using System.Text.Json;
using SocialCare.Application.Interfaces;

namespace SocialCare.Infrastructure.ExternalApis;

public class IbgeClient : IIbgeClient
{
    private readonly HttpClient _http;
    private readonly ILogger<IbgeClient> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public IbgeClient(HttpClient http, ILogger<IbgeClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<IbgeEstado>> ListarEstadosAsync(CancellationToken ct = default)
    {
        try
        {
            var payload = await _http.GetFromJsonAsync<List<EstadoPayload>>("localidades/estados?orderBy=nome", JsonOpts, ct);
            if (payload is null) return Array.Empty<IbgeEstado>();

            return payload
                .Select(e => new IbgeEstado(e.Id, e.Sigla, e.Nome, e.Regiao?.Nome ?? string.Empty))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar estados no IBGE");
            return Array.Empty<IbgeEstado>();
        }
    }

    public async Task<IReadOnlyList<IbgeMunicipio>> ListarMunicipiosAsync(string ufSigla, CancellationToken ct = default)
    {
        try
        {
            var payload = await _http.GetFromJsonAsync<List<MunicipioPayload>>($"localidades/estados/{ufSigla}/municipios?orderBy=nome", JsonOpts, ct);
            if (payload is null) return Array.Empty<IbgeMunicipio>();

            return payload
                .Select(m => new IbgeMunicipio(m.Id, m.Nome))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar municípios da UF {Uf}", ufSigla);
            return Array.Empty<IbgeMunicipio>();
        }
    }

    private class EstadoPayload
    {
        public int Id { get; set; }
        public string Sigla { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public RegiaoPayload? Regiao { get; set; }
    }

    private class RegiaoPayload
    {
        public string Nome { get; set; } = string.Empty;
    }

    private class MunicipioPayload
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}

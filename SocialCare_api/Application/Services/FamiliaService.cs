using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Familias;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class FamiliaService : IFamiliaService
{
    private readonly IRepository<Familia> _familias;
    private readonly IRepository<Municipio> _municipios;
    private readonly IUnitOfWork _uow;
    private readonly IGeocodingClient _geocoding;
    private readonly IValidator<CriarFamiliaRequest> _criarValidator;
    private readonly IValidator<AtualizarFamiliaRequest> _atualizarValidator;

    public FamiliaService(
        IRepository<Familia> familias,
        IRepository<Municipio> municipios,
        IUnitOfWork uow,
        IGeocodingClient geocoding,
        IValidator<CriarFamiliaRequest> criarValidator,
        IValidator<AtualizarFamiliaRequest> atualizarValidator)
    {
        _familias = familias;
        _municipios = municipios;
        _uow = uow;
        _geocoding = geocoding;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<PagedResult<FamiliaResumoDto>> ListarAsync(FamiliaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _familias.Query()
            .Include(f => f.Endereco!).ThenInclude(e => e.Municipio).ThenInclude(m => m.Estado)
            .AsQueryable();

        if (filtro.Status is { } status)
            query = query.Where(f => f.Status == status);

        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var termo = filtro.Busca.Trim();
            query = query.Where(f => f.NomeResponsavel.Contains(termo) || f.CodigoFamiliar.Contains(termo));
        }

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(f => f.NomeResponsavel)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        var dtos = itens.Select(f => f.ToResumoDto()).ToList();
        return new PagedResult<FamiliaResumoDto>(dtos, paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<FamiliaResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var familia = await CarregarCompletaAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Família", id);
        return familia.ToResponse();
    }

    public async Task<FamiliaResponse> CriarAsync(CriarFamiliaRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);
        await GarantirMunicipioAsync(request.Endereco.MunicipioId, ct);

        var codigo = request.CodigoFamiliar.Trim();
        if (await _familias.ExisteAsync(f => f.CodigoFamiliar == codigo, ct))
            throw new ConflictException($"Já existe uma família com o código '{codigo}'.");

        var familia = new Familia
        {
            CodigoFamiliar = codigo,
            NomeResponsavel = request.NomeResponsavel.Trim(),
            Observacoes = request.Observacoes,
            Endereco = MapearEndereco(request.Endereco)
        };

        await GeocodificarEnderecoAsync(familia.Endereco, ct);

        await _familias.AdicionarAsync(familia, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(familia.Id, ct);
    }

    public async Task<FamiliaResponse> AtualizarAsync(int id, AtualizarFamiliaRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);
        await GarantirMunicipioAsync(request.Endereco.MunicipioId, ct);

        var familia = await CarregarCompletaAsync(id, rastrear: true, ct)
            ?? throw new NotFoundException("Família", id);

        familia.NomeResponsavel = request.NomeResponsavel.Trim();
        familia.Status = request.Status;
        familia.Observacoes = request.Observacoes;

        familia.Endereco ??= new Endereco();
        var assinaturaAntes = Assinatura(familia.Endereco);
        AplicarEndereco(familia.Endereco, request.Endereco);

        // Só consulta o geocoder se o endereço mudou (ou ainda não tem coordenadas).
        if (assinaturaAntes != Assinatura(familia.Endereco) || familia.Endereco.Latitude is null)
            await GeocodificarEnderecoAsync(familia.Endereco, ct);

        _familias.Atualizar(familia);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(familia.Id, ct);
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var familia = await _familias.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Família", id);

        // Exclusão lógica: preserva histórico de atendimentos/benefícios.
        familia.Ativo = false;
        familia.Status = Domain.Enums.StatusFamilia.Inativa;
        _familias.Atualizar(familia);
        await _uow.SalvarAlteracoesAsync(ct);
    }

    private Task<Familia?> CarregarCompletaAsync(int id, bool rastrear, CancellationToken ct)
        => _familias.Query(rastrear)
            .Include(f => f.Endereco!).ThenInclude(e => e.Municipio).ThenInclude(m => m.Estado)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

    private async Task GarantirMunicipioAsync(int municipioId, CancellationToken ct)
    {
        if (!await _municipios.ExisteAsync(m => m.Id == municipioId && m.Ativo, ct))
            throw new BusinessException($"Município de id {municipioId} não encontrado. Cadastre-o via importação do IBGE.");
    }

    private static string Assinatura(Endereco e)
        => $"{e.Logradouro}|{e.Numero}|{e.Bairro}|{e.MunicipioId}|{e.Cep}";

    /// <summary>Geocodifica o endereço (Nominatim) e grava lat/lng. Falhas são toleradas (coordenadas ficam nulas).</summary>
    private async Task GeocodificarEnderecoAsync(Endereco endereco, CancellationToken ct)
    {
        var municipio = await _municipios.Query(rastrear: false)
            .Include(m => m.Estado)
            .FirstOrDefaultAsync(m => m.Id == endereco.MunicipioId, ct);
        if (municipio is null) return;

        var coord = await _geocoding.GeocodificarAsync(
            new GeocodingRequest(endereco.Logradouro, endereco.Numero, endereco.Bairro, municipio.Nome, municipio.Estado.Sigla, endereco.Cep),
            ct);

        if (coord is not null)
        {
            endereco.Latitude = coord.Latitude;
            endereco.Longitude = coord.Longitude;
        }
    }

    private static Endereco MapearEndereco(EnderecoRequest req)
    {
        var endereco = new Endereco();
        AplicarEndereco(endereco, req);
        return endereco;
    }

    private static void AplicarEndereco(Endereco endereco, EnderecoRequest req)
    {
        endereco.Cep = req.Cep.Trim();
        endereco.Logradouro = req.Logradouro.Trim();
        endereco.Numero = req.Numero.Trim();
        endereco.Complemento = req.Complemento;
        endereco.Bairro = req.Bairro.Trim();
        endereco.PontoReferencia = req.PontoReferencia;
        endereco.MunicipioId = req.MunicipioId;
    }
}

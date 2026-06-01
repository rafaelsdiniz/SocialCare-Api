using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Visitas;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class VisitaService : IVisitaService
{
    private readonly IRepository<Visita> _visitas;
    private readonly IRepository<Familia> _familias;
    private readonly IBrasilApiClient _brasilApi;
    private readonly IMemoryCache _cache;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<AgendarVisitaRequest> _agendarValidator;
    private readonly IValidator<AtualizarVisitaRequest> _atualizarValidator;
    private readonly IValidator<RegistrarVisitaRequest> _registrarValidator;

    public VisitaService(
        IRepository<Visita> visitas,
        IRepository<Familia> familias,
        IBrasilApiClient brasilApi,
        IMemoryCache cache,
        IUnitOfWork uow,
        IValidator<AgendarVisitaRequest> agendarValidator,
        IValidator<AtualizarVisitaRequest> atualizarValidator,
        IValidator<RegistrarVisitaRequest> registrarValidator)
    {
        _visitas = visitas;
        _familias = familias;
        _brasilApi = brasilApi;
        _cache = cache;
        _uow = uow;
        _agendarValidator = agendarValidator;
        _atualizarValidator = atualizarValidator;
        _registrarValidator = registrarValidator;
    }

    public async Task<PagedResult<VisitaResumoDto>> ListarAsync(VisitaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _visitas.Query()
            .Include(v => v.Familia)
            .Include(v => v.AssistenteResponsavel)
            .AsQueryable();

        if (filtro.FamiliaId is { } fam) query = query.Where(v => v.FamiliaId == fam);
        if (filtro.AssistenteId is { } assist) query = query.Where(v => v.AssistenteResponsavelId == assist);
        if (filtro.Status is { } status) query = query.Where(v => v.Status == status);

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderByDescending(v => v.DataAgendada)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<VisitaResumoDto>(
            itens.Select(v => v.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<VisitaResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var visita = await CarregarAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Visita", id);
        return visita.ToResponse(await AvisoFeriadoAsync(visita.DataAgendada, ct));
    }

    public async Task<VisitaResponse> AgendarAsync(AgendarVisitaRequest request, int? assistenteId, CancellationToken ct = default)
    {
        await _agendarValidator.EnsureValidAsync(request, ct);

        if (assistenteId is not { } assist)
            throw new BusinessException("Não foi possível identificar o assistente responsável (token sem usuário).");

        if (!await _familias.ExisteAsync(f => f.Id == request.FamiliaId && f.Ativo, ct))
            throw new BusinessException($"Família de id {request.FamiliaId} não encontrada ou inativa.");

        var visita = new Visita
        {
            FamiliaId = request.FamiliaId,
            AssistenteResponsavelId = assist,
            DataAgendada = request.DataAgendada,
            Tipo = request.Tipo,
            Motivo = request.Motivo,
            Status = StatusVisita.Agendada
        };

        await _visitas.AdicionarAsync(visita, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(visita.Id, ct);
    }

    public async Task<VisitaResponse> AtualizarAsync(int id, AtualizarVisitaRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var visita = await ObterRastreadaAsync(id, ct);
        if (visita.Status != StatusVisita.Agendada)
            throw new BusinessException("Só é possível editar uma visita agendada.");

        visita.DataAgendada = request.DataAgendada;
        visita.Tipo = request.Tipo;
        visita.Motivo = request.Motivo;
        visita.Observacoes = request.Observacoes;

        _visitas.Atualizar(visita);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<VisitaResponse> RegistrarAsync(int id, RegistrarVisitaRequest request, CancellationToken ct = default)
    {
        await _registrarValidator.EnsureValidAsync(request, ct);

        var visita = await ObterRastreadaAsync(id, ct);
        if (visita.Status is StatusVisita.Cancelada or StatusVisita.Realizada)
            throw new BusinessException("Esta visita já foi realizada ou cancelada.");

        visita.Status = StatusVisita.Realizada;
        visita.DataRealizacao = request.DataRealizacao ?? DateTime.UtcNow;
        visita.Observacoes = request.Observacoes ?? visita.Observacoes;
        visita.Encaminhamentos = request.Encaminhamentos;

        _visitas.Atualizar(visita);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<VisitaResponse> CancelarAsync(int id, CancellationToken ct = default)
    {
        var visita = await ObterRastreadaAsync(id, ct);
        if (visita.Status == StatusVisita.Realizada)
            throw new BusinessException("Não é possível cancelar uma visita já realizada.");

        visita.Status = StatusVisita.Cancelada;
        _visitas.Atualizar(visita);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    private async Task<Visita> ObterRastreadaAsync(int id, CancellationToken ct)
        => await _visitas.ObterPorIdAsync(id, ct) ?? throw new NotFoundException("Visita", id);

    private Task<Visita?> CarregarAsync(int id, bool rastrear, CancellationToken ct)
        => _visitas.Query(rastrear)
            .Include(v => v.Familia)
            .Include(v => v.AssistenteResponsavel)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    /// <summary>Retorna um aviso quando a data agendada coincide com um feriado nacional (BrasilAPI).</summary>
    private async Task<string?> AvisoFeriadoAsync(DateTime data, CancellationToken ct)
    {
        var feriados = await _cache.GetOrCreateAsync($"feriados:{data.Year}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _brasilApi.ListarFeriadosAsync(data.Year, ct);
        });

        var feriado = feriados?.FirstOrDefault(f => f.Data.Date == data.Date);
        return feriado is null ? null : $"Atenção: a data agendada cai no feriado nacional \"{feriado.Nome}\".";
    }
}

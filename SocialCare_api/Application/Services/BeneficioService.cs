using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Beneficios;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class BeneficioService : IBeneficioService
{
    private readonly IRepository<Beneficio> _beneficios;
    private readonly IRepository<Familia> _familias;
    private readonly IRepository<ProgramaSocial> _programas;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<ConcederBeneficioRequest> _concederValidator;
    private readonly IValidator<AtualizarBeneficioRequest> _atualizarValidator;

    public BeneficioService(
        IRepository<Beneficio> beneficios,
        IRepository<Familia> familias,
        IRepository<ProgramaSocial> programas,
        IUnitOfWork uow,
        IValidator<ConcederBeneficioRequest> concederValidator,
        IValidator<AtualizarBeneficioRequest> atualizarValidator)
    {
        _beneficios = beneficios;
        _familias = familias;
        _programas = programas;
        _uow = uow;
        _concederValidator = concederValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<PagedResult<BeneficioResumoDto>> ListarAsync(BeneficioFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _beneficios.Query()
            .Include(b => b.Familia)
            .Include(b => b.ProgramaSocial)
            .AsQueryable();

        if (filtro.FamiliaId is { } fam) query = query.Where(b => b.FamiliaId == fam);
        if (filtro.ProgramaSocialId is { } prog) query = query.Where(b => b.ProgramaSocialId == prog);
        if (filtro.Status is { } status) query = query.Where(b => b.Status == status);

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderByDescending(b => b.DataInicio)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<BeneficioResumoDto>(
            itens.Select(b => b.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<BeneficioResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var beneficio = await CarregarAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Benefício", id);
        return beneficio.ToResponse();
    }

    public async Task<BeneficioResponse> ConcederAsync(ConcederBeneficioRequest request, CancellationToken ct = default)
    {
        await _concederValidator.EnsureValidAsync(request, ct);

        if (!await _familias.ExisteAsync(f => f.Id == request.FamiliaId && f.Ativo, ct))
            throw new BusinessException($"Família de id {request.FamiliaId} não encontrada ou inativa.");

        if (!await _programas.ExisteAsync(p => p.Id == request.ProgramaSocialId && p.Ativo, ct))
            throw new BusinessException($"Programa social de id {request.ProgramaSocialId} não encontrado ou inativo.");

        var beneficio = new Beneficio
        {
            FamiliaId = request.FamiliaId,
            ProgramaSocialId = request.ProgramaSocialId,
            DataInicio = request.DataInicio.Date,
            DataFim = request.DataFim?.Date,
            Valor = request.Valor,
            Observacao = request.Observacao,
            Status = StatusBeneficio.EmAnalise
        };

        await _beneficios.AdicionarAsync(beneficio, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(beneficio.Id, ct);
    }

    public async Task<BeneficioResponse> AtualizarAsync(int id, AtualizarBeneficioRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var beneficio = await ObterRastreadoAsync(id, ct);
        if (beneficio.Status != StatusBeneficio.EmAnalise)
            throw new BusinessException("Só é possível editar um benefício em análise.");

        beneficio.DataInicio = request.DataInicio.Date;
        beneficio.DataFim = request.DataFim?.Date;
        beneficio.Valor = request.Valor;
        beneficio.Observacao = request.Observacao;

        _beneficios.Atualizar(beneficio);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<BeneficioResponse> AprovarAsync(int id, int? aprovadoPorUsuarioId, CancellationToken ct = default)
    {
        var beneficio = await ObterRastreadoAsync(id, ct);
        if (beneficio.Status != StatusBeneficio.EmAnalise)
            throw new BusinessException("Só é possível aprovar um benefício em análise.");

        beneficio.Status = StatusBeneficio.Ativo;
        beneficio.AprovadoPorUsuarioId = aprovadoPorUsuarioId;

        _beneficios.Atualizar(beneficio);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<BeneficioResponse> IndeferirAsync(int id, string? motivo, CancellationToken ct = default)
    {
        var beneficio = await ObterRastreadoAsync(id, ct);
        if (beneficio.Status != StatusBeneficio.EmAnalise)
            throw new BusinessException("Só é possível indeferir um benefício em análise.");

        beneficio.Status = StatusBeneficio.Indeferido;
        beneficio.MotivoEncerramento = motivo;

        _beneficios.Atualizar(beneficio);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<BeneficioResponse> EncerrarAsync(int id, string? motivo, CancellationToken ct = default)
    {
        var beneficio = await ObterRastreadoAsync(id, ct);
        if (beneficio.Status is not (StatusBeneficio.Ativo or StatusBeneficio.Aprovado or StatusBeneficio.Suspenso))
            throw new BusinessException("Apenas benefícios ativos, aprovados ou suspensos podem ser encerrados.");

        beneficio.Status = StatusBeneficio.Encerrado;
        beneficio.MotivoEncerramento = motivo;
        beneficio.DataFim ??= DateTime.Today;

        _beneficios.Atualizar(beneficio);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    private async Task<Beneficio> ObterRastreadoAsync(int id, CancellationToken ct)
        => await _beneficios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Benefício", id);

    private Task<Beneficio?> CarregarAsync(int id, bool rastrear, CancellationToken ct)
        => _beneficios.Query(rastrear)
            .Include(b => b.Familia)
            .Include(b => b.ProgramaSocial)
            .Include(b => b.AprovadoPor)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
}

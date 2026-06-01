using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Encaminhamentos;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class EncaminhamentoService : IEncaminhamentoService
{
    private readonly IRepository<Encaminhamento> _encaminhamentos;
    private readonly IRepository<Familia> _familias;
    private readonly IRepository<InstituicaoParceira> _instituicoes;
    private readonly IRepository<Membro> _membros;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CriarEncaminhamentoRequest> _criarValidator;
    private readonly IValidator<RegistrarRetornoRequest> _retornoValidator;

    public EncaminhamentoService(
        IRepository<Encaminhamento> encaminhamentos,
        IRepository<Familia> familias,
        IRepository<InstituicaoParceira> instituicoes,
        IRepository<Membro> membros,
        IUnitOfWork uow,
        IValidator<CriarEncaminhamentoRequest> criarValidator,
        IValidator<RegistrarRetornoRequest> retornoValidator)
    {
        _encaminhamentos = encaminhamentos;
        _familias = familias;
        _instituicoes = instituicoes;
        _membros = membros;
        _uow = uow;
        _criarValidator = criarValidator;
        _retornoValidator = retornoValidator;
    }

    public async Task<PagedResult<EncaminhamentoResumoDto>> ListarAsync(EncaminhamentoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _encaminhamentos.Query()
            .Include(e => e.Familia)
            .Include(e => e.InstituicaoParceira)
            .AsQueryable();

        if (filtro.FamiliaId is { } fam) query = query.Where(e => e.FamiliaId == fam);
        if (filtro.InstituicaoParceiraId is { } inst) query = query.Where(e => e.InstituicaoParceiraId == inst);
        if (filtro.Status is { } status) query = query.Where(e => e.Status == status);

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderByDescending(e => e.DataEncaminhamento)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<EncaminhamentoResumoDto>(
            itens.Select(e => e.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<EncaminhamentoResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var encaminhamento = await CarregarAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Encaminhamento", id);
        return encaminhamento.ToResponse();
    }

    public async Task<EncaminhamentoResponse> CriarAsync(CriarEncaminhamentoRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);

        if (!await _familias.ExisteAsync(f => f.Id == request.FamiliaId && f.Ativo, ct))
            throw new BusinessException($"Família de id {request.FamiliaId} não encontrada ou inativa.");

        if (!await _instituicoes.ExisteAsync(i => i.Id == request.InstituicaoParceiraId && i.Ativo, ct))
            throw new BusinessException($"Instituição parceira de id {request.InstituicaoParceiraId} não encontrada ou inativa.");

        if (request.MembroId is { } membroId
            && !await _membros.ExisteAsync(m => m.Id == membroId && m.FamiliaId == request.FamiliaId, ct))
            throw new BusinessException($"Membro de id {membroId} não pertence à família informada.");

        var encaminhamento = new Encaminhamento
        {
            FamiliaId = request.FamiliaId,
            InstituicaoParceiraId = request.InstituicaoParceiraId,
            MembroId = request.MembroId,
            Motivo = request.Motivo.Trim(),
            Demanda = request.Demanda,
            DataEncaminhamento = (request.DataEncaminhamento ?? DateTime.UtcNow),
            Status = Domain.Enums.StatusEncaminhamento.Enviado
        };

        await _encaminhamentos.AdicionarAsync(encaminhamento, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(encaminhamento.Id, ct);
    }

    public async Task<EncaminhamentoResponse> RegistrarRetornoAsync(int id, RegistrarRetornoRequest request, CancellationToken ct = default)
    {
        await _retornoValidator.EnsureValidAsync(request, ct);

        var encaminhamento = await _encaminhamentos.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Encaminhamento", id);

        encaminhamento.Status = request.Status;
        encaminhamento.Retorno = request.Retorno;
        encaminhamento.DataRetorno = request.DataRetorno
            ?? (request.Status is Domain.Enums.StatusEncaminhamento.Concluido or Domain.Enums.StatusEncaminhamento.Recusado
                ? DateTime.UtcNow
                : encaminhamento.DataRetorno);

        _encaminhamentos.Atualizar(encaminhamento);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    private Task<Encaminhamento?> CarregarAsync(int id, bool rastrear, CancellationToken ct)
        => _encaminhamentos.Query(rastrear)
            .Include(e => e.Familia)
            .Include(e => e.InstituicaoParceira)
            .Include(e => e.Membro)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Atendimentos;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class AtendimentoService : IAtendimentoService
{
    private readonly IRepository<Atendimento> _atendimentos;
    private readonly IRepository<Familia> _familias;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<AbrirAtendimentoRequest> _abrirValidator;
    private readonly IValidator<AtualizarAtendimentoRequest> _atualizarValidator;

    public AtendimentoService(
        IRepository<Atendimento> atendimentos,
        IRepository<Familia> familias,
        IUnitOfWork uow,
        IValidator<AbrirAtendimentoRequest> abrirValidator,
        IValidator<AtualizarAtendimentoRequest> atualizarValidator)
    {
        _atendimentos = atendimentos;
        _familias = familias;
        _uow = uow;
        _abrirValidator = abrirValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<PagedResult<AtendimentoResumoDto>> ListarAsync(AtendimentoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _atendimentos.Query()
            .Include(a => a.Familia)
            .Include(a => a.AssistenteResponsavel)
            .AsQueryable();

        if (filtro.FamiliaId is { } fam) query = query.Where(a => a.FamiliaId == fam);
        if (filtro.AssistenteId is { } assist) query = query.Where(a => a.AssistenteResponsavelId == assist);
        if (filtro.Status is { } status) query = query.Where(a => a.Status == status);

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderByDescending(a => a.DataAtendimento)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<AtendimentoResumoDto>(
            itens.Select(a => a.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<AtendimentoResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var atendimento = await CarregarAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Atendimento", id);
        return atendimento.ToResponse();
    }

    public async Task<AtendimentoResponse> AbrirAsync(AbrirAtendimentoRequest request, int? assistenteId, CancellationToken ct = default)
    {
        await _abrirValidator.EnsureValidAsync(request, ct);

        if (assistenteId is not { } assist)
            throw new BusinessException("Não foi possível identificar o assistente responsável (token sem usuário).");

        if (!await _familias.ExisteAsync(f => f.Id == request.FamiliaId && f.Ativo, ct))
            throw new BusinessException($"Família de id {request.FamiliaId} não encontrada ou inativa.");

        var atendimento = new Atendimento
        {
            FamiliaId = request.FamiliaId,
            AssistenteResponsavelId = assist,
            DataAtendimento = request.DataAtendimento ?? DateTime.UtcNow,
            Motivo = request.Motivo.Trim(),
            Demanda = request.Demanda,
            Remoto = request.Remoto,
            Status = StatusAtendimento.Aberto
        };

        await _atendimentos.AdicionarAsync(atendimento, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(atendimento.Id, ct);
    }

    public async Task<AtendimentoResponse> AtualizarAsync(int id, AtualizarAtendimentoRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var atendimento = await _atendimentos.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Atendimento", id);

        if (atendimento.Status is StatusAtendimento.Concluido or StatusAtendimento.Cancelado)
            throw new BusinessException("Atendimentos concluídos ou cancelados não podem ser alterados.");

        atendimento.Motivo = request.Motivo.Trim();
        atendimento.Demanda = request.Demanda;
        atendimento.Parecer = request.Parecer;
        atendimento.Remoto = request.Remoto;
        atendimento.Status = request.Status;

        _atendimentos.Atualizar(atendimento);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    private Task<Atendimento?> CarregarAsync(int id, bool rastrear, CancellationToken ct)
        => _atendimentos.Query(rastrear)
            .Include(a => a.Familia)
            .Include(a => a.AssistenteResponsavel)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
}

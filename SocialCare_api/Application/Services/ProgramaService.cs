using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Programas;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class ProgramaService : IProgramaService
{
    private readonly IRepository<ProgramaSocial> _programas;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CriarProgramaRequest> _criarValidator;
    private readonly IValidator<AtualizarProgramaRequest> _atualizarValidator;

    public ProgramaService(
        IRepository<ProgramaSocial> programas,
        IUnitOfWork uow,
        IValidator<CriarProgramaRequest> criarValidator,
        IValidator<AtualizarProgramaRequest> atualizarValidator)
    {
        _programas = programas;
        _uow = uow;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<PagedResult<ProgramaResumoDto>> ListarAsync(ProgramaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _programas.Query();

        if (filtro.Ativo is { } ativo)
            query = query.Where(p => p.Ativo == ativo);

        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var termo = filtro.Busca.Trim();
            query = query.Where(p => p.Nome.Contains(termo) || p.OrgaoResponsavel.Contains(termo));
        }

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(p => p.Nome)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<ProgramaResumoDto>(
            itens.Select(p => p.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<ProgramaResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var programa = await _programas.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Programa social", id);
        return programa.ToResponse();
    }

    public async Task<ProgramaResponse> CriarAsync(CriarProgramaRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);

        var nome = request.Nome.Trim();
        if (await _programas.ExisteAsync(p => p.Nome == nome, ct))
            throw new ConflictException($"Já existe um programa com o nome '{nome}'.");

        var programa = new ProgramaSocial
        {
            Nome = nome,
            OrgaoResponsavel = request.OrgaoResponsavel.Trim(),
            Descricao = request.Descricao,
            Requisitos = request.Requisitos,
            ValorPadrao = request.ValorPadrao,
            DuracaoMesesPadrao = request.DuracaoMesesPadrao,
            VigenciaInicio = request.VigenciaInicio,
            VigenciaFim = request.VigenciaFim
        };

        await _programas.AdicionarAsync(programa, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return programa.ToResponse();
    }

    public async Task<ProgramaResponse> AtualizarAsync(int id, AtualizarProgramaRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var programa = await _programas.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Programa social", id);

        var nome = request.Nome.Trim();
        if (!string.Equals(programa.Nome, nome, StringComparison.OrdinalIgnoreCase)
            && await _programas.ExisteAsync(p => p.Nome == nome && p.Id != id, ct))
            throw new ConflictException($"Já existe um programa com o nome '{nome}'.");

        programa.Nome = nome;
        programa.OrgaoResponsavel = request.OrgaoResponsavel.Trim();
        programa.Descricao = request.Descricao;
        programa.Requisitos = request.Requisitos;
        programa.ValorPadrao = request.ValorPadrao;
        programa.DuracaoMesesPadrao = request.DuracaoMesesPadrao;
        programa.VigenciaInicio = request.VigenciaInicio;
        programa.VigenciaFim = request.VigenciaFim;
        programa.Ativo = request.Ativo;

        _programas.Atualizar(programa);
        await _uow.SalvarAlteracoesAsync(ct);

        return programa.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var programa = await _programas.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Programa social", id);

        programa.Ativo = false;
        _programas.Atualizar(programa);
        await _uow.SalvarAlteracoesAsync(ct);
    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Membros;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class MembroService : IMembroService
{
    private readonly IRepository<Membro> _membros;
    private readonly IRepository<Familia> _familias;
    private readonly IRepository<Parentesco> _parentescos;
    private readonly IRepository<TipoDocumento> _tiposDocumento;
    private readonly IRepository<TipoRenda> _tiposRenda;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CriarMembroRequest> _criarValidator;
    private readonly IValidator<AtualizarMembroRequest> _atualizarValidator;

    public MembroService(
        IRepository<Membro> membros,
        IRepository<Familia> familias,
        IRepository<Parentesco> parentescos,
        IRepository<TipoDocumento> tiposDocumento,
        IRepository<TipoRenda> tiposRenda,
        IUnitOfWork uow,
        IValidator<CriarMembroRequest> criarValidator,
        IValidator<AtualizarMembroRequest> atualizarValidator)
    {
        _membros = membros;
        _familias = familias;
        _parentescos = parentescos;
        _tiposDocumento = tiposDocumento;
        _tiposRenda = tiposRenda;
        _uow = uow;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<IReadOnlyList<MembroResumoDto>> ListarPorFamiliaAsync(int familiaId, CancellationToken ct = default)
    {
        await GarantirFamiliaAsync(familiaId, ct);
        var membros = await _membros.Query()
            .Where(m => m.FamiliaId == familiaId && m.Ativo)
            .Include(m => m.Parentesco)
            .Include(m => m.Rendas).ThenInclude(r => r.TipoRenda)
            .OrderBy(m => m.Nome)
            .ToListAsync(ct);

        return membros.Select(m => m.ToResumoDto()).ToList();
    }

    public async Task<MembroResponse> ObterAsync(int familiaId, int membroId, CancellationToken ct = default)
    {
        var membro = await CarregarAsync(familiaId, membroId, rastrear: false, ct)
            ?? throw new NotFoundException("Membro", membroId);
        return membro.ToResponse();
    }

    public async Task<MembroResponse> CriarAsync(int familiaId, CriarMembroRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);
        await GarantirFamiliaAsync(familiaId, ct);
        await GarantirParentescoAsync(request.ParentescoId, ct);
        await GarantirTiposAsync(request.Documentos, request.Rendas, ct);

        var membro = new Membro { FamiliaId = familiaId };
        AplicarEscalares(membro, request.Nome, request.DataNascimento, request.Sexo, request.EstadoCivil,
            request.ParentescoId, request.NomeMae, request.NomePai, request.Escolaridade, request.Ocupacao,
            request.PessoaComDeficiencia, request.DescricaoDeficiencia, request.Telefone);
        membro.Documentos = ConstruirDocumentos(request.Documentos);
        membro.Rendas = ConstruirRendas(request.Rendas);

        await _membros.AdicionarAsync(membro, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        await RecalcularFamiliaAsync(familiaId, ct);
        return await ObterAsync(familiaId, membro.Id, ct);
    }

    public async Task<MembroResponse> AtualizarAsync(int familiaId, int membroId, AtualizarMembroRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);
        await GarantirParentescoAsync(request.ParentescoId, ct);
        await GarantirTiposAsync(request.Documentos, request.Rendas, ct);

        var membro = await CarregarAsync(familiaId, membroId, rastrear: true, ct)
            ?? throw new NotFoundException("Membro", membroId);

        AplicarEscalares(membro, request.Nome, request.DataNascimento, request.Sexo, request.EstadoCivil,
            request.ParentescoId, request.NomeMae, request.NomePai, request.Escolaridade, request.Ocupacao,
            request.PessoaComDeficiencia, request.DescricaoDeficiencia, request.Telefone);

        // Substitui as coleções (estratégia de replace simples).
        membro.Documentos.Clear();
        foreach (var d in ConstruirDocumentos(request.Documentos)) membro.Documentos.Add(d);
        membro.Rendas.Clear();
        foreach (var r in ConstruirRendas(request.Rendas)) membro.Rendas.Add(r);

        _membros.Atualizar(membro);
        await _uow.SalvarAlteracoesAsync(ct);

        await RecalcularFamiliaAsync(familiaId, ct);
        return await ObterAsync(familiaId, membro.Id, ct);
    }

    public async Task RemoverAsync(int familiaId, int membroId, CancellationToken ct = default)
    {
        var membro = await _membros.Query(rastrear: true)
            .FirstOrDefaultAsync(m => m.Id == membroId && m.FamiliaId == familiaId, ct)
            ?? throw new NotFoundException("Membro", membroId);

        membro.Ativo = false;
        _membros.Atualizar(membro);
        await _uow.SalvarAlteracoesAsync(ct);

        await RecalcularFamiliaAsync(familiaId, ct);
    }

    /// <summary>Recalcula quantidade de membros, renda total e per capita da família.</summary>
    private async Task RecalcularFamiliaAsync(int familiaId, CancellationToken ct)
    {
        var familia = await _familias.Query(rastrear: true).FirstOrDefaultAsync(f => f.Id == familiaId, ct);
        if (familia is null) return;

        var membros = await _membros.Query()
            .Where(m => m.FamiliaId == familiaId && m.Ativo)
            .Include(m => m.Rendas).ThenInclude(r => r.TipoRenda)
            .ToListAsync(ct);

        familia.QuantidadeMembros = membros.Count;
        familia.RendaTotalMensal = membros.Sum(m => m.RendaConsiderada());
        familia.RendaPerCapita = membros.Count > 0
            ? Math.Round(familia.RendaTotalMensal / membros.Count, 2)
            : 0m;

        _familias.Atualizar(familia);
        await _uow.SalvarAlteracoesAsync(ct);
    }

    private Task<Membro?> CarregarAsync(int familiaId, int membroId, bool rastrear, CancellationToken ct)
        => _membros.Query(rastrear)
            .Include(m => m.Parentesco)
            .Include(m => m.Documentos).ThenInclude(d => d.TipoDocumento)
            .Include(m => m.Rendas).ThenInclude(r => r.TipoRenda)
            .FirstOrDefaultAsync(m => m.Id == membroId && m.FamiliaId == familiaId, ct);

    private async Task GarantirFamiliaAsync(int familiaId, CancellationToken ct)
    {
        if (!await _familias.ExisteAsync(f => f.Id == familiaId, ct))
            throw new NotFoundException("Família", familiaId);
    }

    private async Task GarantirParentescoAsync(int parentescoId, CancellationToken ct)
    {
        if (!await _parentescos.ExisteAsync(p => p.Id == parentescoId && p.Ativo, ct))
            throw new BusinessException($"Parentesco de id {parentescoId} não encontrado.");
    }

    private async Task GarantirTiposAsync(IReadOnlyList<DocumentoRequest>? documentos, IReadOnlyList<RendaRequest>? rendas, CancellationToken ct)
    {
        foreach (var tipoId in (documentos ?? []).Select(d => d.TipoDocumentoId).Distinct())
            if (!await _tiposDocumento.ExisteAsync(t => t.Id == tipoId, ct))
                throw new BusinessException($"Tipo de documento de id {tipoId} não encontrado.");

        foreach (var tipoId in (rendas ?? []).Select(r => r.TipoRendaId).Distinct())
            if (!await _tiposRenda.ExisteAsync(t => t.Id == tipoId, ct))
                throw new BusinessException($"Tipo de renda de id {tipoId} não encontrado.");
    }

    private static void AplicarEscalares(Membro m, string nome, DateTime nascimento, Domain.Enums.Sexo sexo,
        Domain.Enums.EstadoCivil estadoCivil, int parentescoId, string? nomeMae, string? nomePai,
        string? escolaridade, string? ocupacao, bool pcd, string? descricaoDeficiencia, string? telefone)
    {
        m.Nome = nome.Trim();
        m.DataNascimento = nascimento.Date;
        m.Sexo = sexo;
        m.EstadoCivil = estadoCivil;
        m.ParentescoId = parentescoId;
        m.NomeMae = nomeMae;
        m.NomePai = nomePai;
        m.Escolaridade = escolaridade;
        m.Ocupacao = ocupacao;
        m.PessoaComDeficiencia = pcd;
        m.DescricaoDeficiencia = pcd ? descricaoDeficiencia : null;
        m.Telefone = telefone;
    }

    private static List<Documento> ConstruirDocumentos(IReadOnlyList<DocumentoRequest>? documentos)
        => (documentos ?? []).Select(d => new Documento
        {
            TipoDocumentoId = d.TipoDocumentoId,
            Numero = d.Numero.Trim(),
            OrgaoEmissor = d.OrgaoEmissor,
            DataEmissao = d.DataEmissao,
            DataValidade = d.DataValidade
        }).ToList();

    private static List<Renda> ConstruirRendas(IReadOnlyList<RendaRequest>? rendas)
        => (rendas ?? []).Select(r => new Renda
        {
            TipoRendaId = r.TipoRendaId,
            Valor = r.Valor,
            MesReferencia = r.MesReferencia,
            AnoReferencia = r.AnoReferencia,
            Fonte = r.Fonte,
            Observacao = r.Observacao
        }).ToList();
}

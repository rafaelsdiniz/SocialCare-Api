using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Externos;
using SocialCare.Application.DTOs.Instituicoes;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class InstituicaoService : IInstituicaoService
{
    private readonly IRepository<InstituicaoParceira> _instituicoes;
    private readonly IBrasilApiClient _brasilApi;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CriarInstituicaoRequest> _criarValidator;
    private readonly IValidator<AtualizarInstituicaoRequest> _atualizarValidator;

    public InstituicaoService(
        IRepository<InstituicaoParceira> instituicoes,
        IBrasilApiClient brasilApi,
        IUnitOfWork uow,
        IValidator<CriarInstituicaoRequest> criarValidator,
        IValidator<AtualizarInstituicaoRequest> atualizarValidator)
    {
        _instituicoes = instituicoes;
        _brasilApi = brasilApi;
        _uow = uow;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<PagedResult<InstituicaoResumoDto>> ListarAsync(InstituicaoFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _instituicoes.Query();

        if (filtro.Ativo is { } ativo) query = query.Where(i => i.Ativo == ativo);
        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var termo = filtro.Busca.Trim();
            query = query.Where(i => i.Nome.Contains(termo) || i.AreaAtuacao.Contains(termo) || i.Cnpj.Contains(termo));
        }

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(i => i.Nome)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<InstituicaoResumoDto>(
            itens.Select(i => i.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<InstituicaoResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var instituicao = await _instituicoes.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Instituição parceira", id);
        return instituicao.ToResponse();
    }

    public async Task<InstituicaoResponse> CriarAsync(CriarInstituicaoRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);

        var cnpj = DocumentoFiscal.FormatarCnpj(request.Cnpj);
        if (await _instituicoes.ExisteAsync(i => i.Cnpj == cnpj, ct))
            throw new ConflictException($"Já existe uma instituição com o CNPJ '{cnpj}'.");

        var instituicao = new InstituicaoParceira
        {
            Nome = request.Nome.Trim(),
            Cnpj = cnpj,
            AreaAtuacao = request.AreaAtuacao.Trim(),
            Telefone = request.Telefone,
            Email = request.Email,
            ResponsavelContato = request.ResponsavelContato,
            EnderecoCompleto = request.EnderecoCompleto
        };

        await _instituicoes.AdicionarAsync(instituicao, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return instituicao.ToResponse();
    }

    public async Task<InstituicaoResponse> AtualizarAsync(int id, AtualizarInstituicaoRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var instituicao = await _instituicoes.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Instituição parceira", id);

        var cnpj = DocumentoFiscal.FormatarCnpj(request.Cnpj);
        if (instituicao.Cnpj != cnpj && await _instituicoes.ExisteAsync(i => i.Cnpj == cnpj && i.Id != id, ct))
            throw new ConflictException($"Já existe uma instituição com o CNPJ '{cnpj}'.");

        instituicao.Nome = request.Nome.Trim();
        instituicao.Cnpj = cnpj;
        instituicao.AreaAtuacao = request.AreaAtuacao.Trim();
        instituicao.Telefone = request.Telefone;
        instituicao.Email = request.Email;
        instituicao.ResponsavelContato = request.ResponsavelContato;
        instituicao.EnderecoCompleto = request.EnderecoCompleto;
        instituicao.Ativo = request.Ativo;

        _instituicoes.Atualizar(instituicao);
        await _uow.SalvarAlteracoesAsync(ct);

        return instituicao.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var instituicao = await _instituicoes.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Instituição parceira", id);

        instituicao.Ativo = false;
        _instituicoes.Atualizar(instituicao);
        await _uow.SalvarAlteracoesAsync(ct);
    }

    public async Task<CnpjResponse> ConsultarCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        if (!DocumentoFiscal.CnpjValido(cnpj))
            throw new BusinessException("CNPJ inválido (dígitos verificadores não conferem).");

        return await _brasilApi.ConsultarCnpjAsync(cnpj, ct)
            ?? throw new NotFoundException($"CNPJ '{DocumentoFiscal.FormatarCnpj(cnpj)}' não encontrado na BrasilAPI.");
    }
}

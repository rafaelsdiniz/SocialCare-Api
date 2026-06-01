using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.Common.Exceptions;
using SocialCare.Application.DTOs.Usuarios;
using SocialCare.Application.Interfaces;
using SocialCare.Application.Mappings;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IRepository<Usuario> _usuarios;
    private readonly IRepository<Perfil> _perfis;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CriarUsuarioRequest> _criarValidator;
    private readonly IValidator<AtualizarUsuarioRequest> _atualizarValidator;
    private readonly IValidator<AlterarSenhaRequest> _senhaValidator;

    public UsuarioService(
        IRepository<Usuario> usuarios,
        IRepository<Perfil> perfis,
        IPasswordHasher<Usuario> hasher,
        IUnitOfWork uow,
        IValidator<CriarUsuarioRequest> criarValidator,
        IValidator<AtualizarUsuarioRequest> atualizarValidator,
        IValidator<AlterarSenhaRequest> senhaValidator)
    {
        _usuarios = usuarios;
        _perfis = perfis;
        _hasher = hasher;
        _uow = uow;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
        _senhaValidator = senhaValidator;
    }

    public async Task<PagedResult<UsuarioResumoDto>> ListarAsync(UsuarioFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _usuarios.Query()
            .Include(u => u.UsuarioPerfis).ThenInclude(up => up.Perfil)
            .AsQueryable();

        if (filtro.Ativo is { } ativo) query = query.Where(u => u.Ativo == ativo);
        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var termo = filtro.Busca.Trim();
            query = query.Where(u => u.Nome.Contains(termo) || u.Login.Contains(termo) || u.Email.Contains(termo));
        }

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(u => u.Nome)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync(ct);

        return new PagedResult<UsuarioResumoDto>(
            itens.Select(u => u.ToResumoDto()).ToList(),
            paginacao.Pagina, paginacao.TamanhoPagina, total);
    }

    public async Task<UsuarioResponse> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var usuario = await CarregarAsync(id, rastrear: false, ct)
            ?? throw new NotFoundException("Usuário", id);
        return usuario.ToResponse();
    }

    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken ct = default)
    {
        await _criarValidator.EnsureValidAsync(request, ct);

        var login = request.Login.Trim();
        var email = request.Email.Trim();

        if (await _usuarios.ExisteAsync(u => u.Login == login, ct))
            throw new ConflictException($"Já existe um usuário com o login '{login}'.");
        if (await _usuarios.ExisteAsync(u => u.Email == email, ct))
            throw new ConflictException($"Já existe um usuário com o e-mail '{email}'.");

        await GarantirPerfisAsync(request.PerfilIds, ct);

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            Login = login
        };
        usuario.SenhaHash = _hasher.HashPassword(usuario, request.Senha);
        usuario.UsuarioPerfis = MontarPerfis(request.PerfilIds);

        await _usuarios.AdicionarAsync(usuario, ct);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(usuario.Id, ct);
    }

    public async Task<UsuarioResponse> AtualizarAsync(int id, AtualizarUsuarioRequest request, CancellationToken ct = default)
    {
        await _atualizarValidator.EnsureValidAsync(request, ct);

        var usuario = await CarregarAsync(id, rastrear: true, ct)
            ?? throw new NotFoundException("Usuário", id);

        var email = request.Email.Trim();
        if (usuario.Email != email && await _usuarios.ExisteAsync(u => u.Email == email && u.Id != id, ct))
            throw new ConflictException($"Já existe um usuário com o e-mail '{email}'.");

        await GarantirPerfisAsync(request.PerfilIds, ct);

        usuario.Nome = request.Nome.Trim();
        usuario.Email = email;
        usuario.Ativo = request.Ativo;

        usuario.UsuarioPerfis.Clear();
        foreach (var up in MontarPerfis(request.PerfilIds)) usuario.UsuarioPerfis.Add(up);

        _usuarios.Atualizar(usuario);
        await _uow.SalvarAlteracoesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task AlterarSenhaAsync(int id, AlterarSenhaRequest request, CancellationToken ct = default)
    {
        await _senhaValidator.EnsureValidAsync(request, ct);

        var usuario = await _usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Usuário", id);

        usuario.SenhaHash = _hasher.HashPassword(usuario, request.NovaSenha);
        _usuarios.Atualizar(usuario);
        await _uow.SalvarAlteracoesAsync(ct);
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException("Usuário", id);

        usuario.Ativo = false;
        _usuarios.Atualizar(usuario);
        await _uow.SalvarAlteracoesAsync(ct);
    }

    private async Task GarantirPerfisAsync(IReadOnlyList<int> perfilIds, CancellationToken ct)
    {
        foreach (var perfilId in perfilIds.Distinct())
            if (!await _perfis.ExisteAsync(p => p.Id == perfilId && p.Ativo, ct))
                throw new BusinessException($"Perfil de id {perfilId} não encontrado.");
    }

    private static List<UsuarioPerfil> MontarPerfis(IReadOnlyList<int> perfilIds)
        => perfilIds.Distinct()
            .Select(pid => new UsuarioPerfil { PerfilId = pid, AtribuidoEm = DateTime.UtcNow })
            .ToList();

    private Task<Usuario?> CarregarAsync(int id, bool rastrear, CancellationToken ct)
        => _usuarios.Query(rastrear)
            .Include(u => u.UsuarioPerfis).ThenInclude(up => up.Perfil)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
}

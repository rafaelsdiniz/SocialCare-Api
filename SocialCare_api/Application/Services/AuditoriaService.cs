using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Auditoria;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;

namespace SocialCare.Application.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly IRepository<LogAuditoria> _logs;

    public AuditoriaService(IRepository<LogAuditoria> logs)
    {
        _logs = logs;
    }

    public async Task<PagedResult<LogAuditoriaDto>> ListarAsync(AuditoriaFiltro filtro, PaginacaoQuery paginacao, CancellationToken ct = default)
    {
        var query = _logs.Query().Include(l => l.Usuario).AsQueryable();

        if (filtro.UsuarioId is { } usuarioId) query = query.Where(l => l.UsuarioId == usuarioId);
        if (filtro.Tipo is { } tipo) query = query.Where(l => l.Tipo == tipo);
        if (!string.IsNullOrWhiteSpace(filtro.Entidade))
        {
            var termo = filtro.Entidade.Trim();
            query = query.Where(l => l.Entidade == termo);
        }

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderByDescending(l => l.CriadoEm)
            .Skip(paginacao.Skip)
            .Take(paginacao.TamanhoPagina)
            .Select(l => new LogAuditoriaDto(
                l.Id,
                l.UsuarioId,
                l.Usuario != null ? l.Usuario.Nome : null,
                l.Tipo.ToString(),
                l.Entidade,
                l.EntidadeId,
                l.DadosDepois,
                l.EnderecoIp,
                l.UserAgent,
                l.CriadoEm))
            .ToListAsync(ct);

        return new PagedResult<LogAuditoriaDto>(itens, paginacao.Pagina, paginacao.TamanhoPagina, total);
    }
}

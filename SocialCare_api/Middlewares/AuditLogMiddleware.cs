using SocialCare.Application.Common;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Middlewares;

/// <summary>Registra na trilha de auditoria as requisições mutantes (POST/PUT/PATCH/DELETE) bem-sucedidas.</summary>
public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;

    public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (!DeveAuditar(context)) return;

        try
        {
            await RegistrarAsync(context);
        }
        catch (Exception ex)
        {
            // Auditoria nunca deve quebrar a requisição.
            _logger.LogWarning(ex, "Falha ao gravar log de auditoria para {Path}", context.Request.Path);
        }
    }

    private static bool DeveAuditar(HttpContext context)
    {
        var metodo = context.Request.Method;
        var ehMutacao = HttpMethods.IsPost(metodo) || HttpMethods.IsPut(metodo)
            || HttpMethods.IsPatch(metodo) || HttpMethods.IsDelete(metodo);

        var path = context.Request.Path.Value ?? string.Empty;
        var ehApi = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
        var sucesso = context.Response.StatusCode is >= 200 and < 300;

        return ehMutacao && ehApi && sucesso;
    }

    private async Task RegistrarAsync(HttpContext context)
    {
        var db = context.RequestServices.GetRequiredService<AppDbContext>();
        var path = context.Request.Path.Value ?? string.Empty;
        var (entidade, entidadeId) = ExtrairEntidade(path);

        var log = new LogAuditoria
        {
            UsuarioId = context.User.ObterUsuarioId(),
            Tipo = MapearTipo(context.Request.Method, path),
            Entidade = entidade,
            EntidadeId = entidadeId,
            DadosDepois = $"{context.Request.Method} {path} -> {context.Response.StatusCode}",
            EnderecoIp = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString() is { Length: > 0 } ua
                ? ua[..Math.Min(ua.Length, 255)]
                : null
        };

        db.LogsAuditoria.Add(log);
        await db.SaveChangesAsync();
    }

    private static TipoAuditoria MapearTipo(string metodo, string path)
    {
        if (path.Contains("auth/login", StringComparison.OrdinalIgnoreCase))
            return TipoAuditoria.Login;

        if (HttpMethods.IsDelete(metodo)) return TipoAuditoria.Exclusao;
        if (HttpMethods.IsPost(metodo)) return TipoAuditoria.Criacao;
        return TipoAuditoria.Alteracao;
    }

    private static (string Entidade, string? EntidadeId) ExtrairEntidade(string path)
    {
        var segmentos = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        // ["api", "<entidade>", "<id?>", ...]
        var entidade = segmentos.Length >= 2 ? segmentos[1] : "desconhecida";
        var id = segmentos.Skip(2).FirstOrDefault(s => s.All(char.IsDigit));
        return (entidade, id);
    }
}

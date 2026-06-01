using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SocialCare.Application.Common.Exceptions;

namespace SocialCare.Middlewares;

/// <summary>Captura exceções não tratadas e devolve uma resposta padronizada (ProblemDetails).</summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarAsync(context, ex);
        }
    }

    private async Task TratarAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problema = ex switch
        {
            ValidationException v => Criar(StatusCodes.Status400BadRequest, "Dados inválidos.", v.Message, v.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
            NotFoundException nf => Criar(StatusCodes.Status404NotFound, "Recurso não encontrado.", nf.Message),
            ConflictException cf => Criar(StatusCodes.Status409Conflict, "Conflito de dados.", cf.Message),
            BusinessException be => Criar(StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada.", be.Message),
            _ => Criar(StatusCodes.Status500InternalServerError, "Erro interno.",
                _env.IsDevelopment() ? ex.ToString() : "Ocorreu um erro inesperado. Tente novamente.")
        };

        if (problema.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);

        context.Response.StatusCode = problema.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problema, JsonOptions));
    }

    private static ProblemDetails Criar(int status, string titulo, string detalhe, IDictionary<string, string[]>? erros = null)
    {
        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe
        };
        if (erros is not null) problema.Extensions["erros"] = erros;
        return problema;
    }
}

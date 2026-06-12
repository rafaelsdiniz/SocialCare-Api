using FluentValidation;

namespace SocialCare.Application.Common;

public static class ValidatorExtensions
{
    /// <summary>
    /// Valida a requisição lançando <see cref="ValidationException"/> (→ 400) também quando o corpo
    /// é nulo/JSON malformado, evitando que um corpo ausente resulte em erro 500.
    /// </summary>
    public static Task EnsureValidAsync<T>(this IValidator<T> validator, T? instance, CancellationToken ct)
    {
        if (instance is null)
            throw new ValidationException("O corpo da requisição é obrigatório e deve ser um JSON válido.");

        return validator.ValidateAndThrowAsync(instance, ct);
    }
}

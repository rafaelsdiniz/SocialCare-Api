namespace SocialCare.Application.Common.Exceptions;

/// <summary>Lançada quando um recurso solicitado não é encontrado (HTTP 404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem) { }

    public NotFoundException(string recurso, object chave)
        : base($"{recurso} com identificador '{chave}' não foi encontrado(a).") { }
}

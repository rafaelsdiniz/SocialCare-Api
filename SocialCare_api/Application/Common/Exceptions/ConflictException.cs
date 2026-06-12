namespace SocialCare.Application.Common.Exceptions;

/// <summary>Lançada quando há conflito com o estado atual do recurso, ex.: duplicidade (HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string mensagem) : base(mensagem) { }
}

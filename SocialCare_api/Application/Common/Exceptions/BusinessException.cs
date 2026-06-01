namespace SocialCare.Application.Common.Exceptions;

/// <summary>Lançada quando uma regra de negócio é violada (HTTP 422).</summary>
public class BusinessException : Exception
{
    public BusinessException(string mensagem) : base(mensagem) { }
}

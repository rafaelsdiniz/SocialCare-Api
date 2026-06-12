namespace SocialCare.Application.Common.Exceptions;

/// <summary>Lançada quando um serviço externo (BrasilAPI, ViaCEP, etc.) falha ou está indisponível (HTTP 503).</summary>
public class ExternalServiceException : Exception
{
    public ExternalServiceException(string mensagem) : base(mensagem) { }

    public ExternalServiceException(string mensagem, Exception innerException) : base(mensagem, innerException) { }
}

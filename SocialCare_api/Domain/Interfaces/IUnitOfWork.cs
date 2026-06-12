namespace SocialCare.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SalvarAlteracoesAsync(CancellationToken ct = default);
}

using System.Linq.Expressions;
using SocialCare.Domain.Entities;

namespace SocialCare.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default);
    Task AdicionarAsync(T entidade, CancellationToken ct = default);
    void Atualizar(T entidade);
    void Remover(T entidade);

    /// <summary>Consulta base para projeções/includes/paginação na camada de aplicação.</summary>
    IQueryable<T> Query(bool rastrear = false);
}

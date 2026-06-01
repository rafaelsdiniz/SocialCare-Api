using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Interfaces;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default)
        => await Set.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default)
        => await Set.AsNoTracking().Where(predicado).ToListAsync(ct);

    public Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default)
        => Set.AnyAsync(predicado, ct);

    public async Task AdicionarAsync(T entidade, CancellationToken ct = default)
        => await Set.AddAsync(entidade, ct);

    public void Atualizar(T entidade) => Set.Update(entidade);

    public void Remover(T entidade) => Set.Remove(entidade);

    public IQueryable<T> Query(bool rastrear = false)
        => rastrear ? Set : Set.AsNoTracking();
}

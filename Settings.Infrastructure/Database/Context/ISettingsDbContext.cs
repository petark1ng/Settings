using Microsoft.EntityFrameworkCore;

namespace Settings.Infrastructure.Database.Context;
public interface ISettingsDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}

using Microsoft.EntityFrameworkCore;
using Settings.Entities;
using Settings.Infrastructure.Database.Context;

namespace Settings.Infrastructure.Repositories;
public class RepositoryBase
{
    protected readonly ISettingsDbContext _settingsDbContext;

    public RepositoryBase(ISettingsDbContext settingsDbContext)
    {
        _settingsDbContext = settingsDbContext;
    }

    protected IQueryable<T> All<T>() where T : BaseEntity
    {
        return _settingsDbContext.Set<T>();
    }

    protected IQueryable<T> AllWithoutTracking<T>() where T : BaseEntity
    {
        return _settingsDbContext.Set<T>().AsNoTracking();
    }

    protected void Insert<T>(T entity) where T : BaseEntity
    {
        _settingsDbContext.Set<T>().Add(entity);
    }

    protected void Delete<T>(T entity) where T : BaseEntity
    {
        _settingsDbContext.Set<T>().Remove(entity);
    }
}

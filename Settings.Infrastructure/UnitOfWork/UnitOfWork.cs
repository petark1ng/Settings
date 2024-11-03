using Settings.Contracts.Interfaces;
using Settings.Infrastructure.Database.Context;

namespace Settings.Infrastructure.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly ISettingsDbContext _settingsDbContext;

    public UnitOfWork(ISettingsDbContext settingsDbContext)
    {
        _settingsDbContext = settingsDbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _settingsDbContext.SaveChangesAsync();
    }
}

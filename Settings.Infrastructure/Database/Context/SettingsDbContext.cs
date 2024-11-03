using Microsoft.EntityFrameworkCore;
using Settings.Infrastructure.Database.Configuration;

namespace Settings.Infrastructure.Database.Context;
public class SettingsDbContext : DbContext, ISettingsDbContext
{
    public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SettingConfiguration());
        modelBuilder.ApplyConfiguration(new SettingValueConfiguration());
    }
}

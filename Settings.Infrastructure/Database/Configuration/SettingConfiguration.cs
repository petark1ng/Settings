using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Settings.Entities;

namespace Settings.Infrastructure.Database.Configuration;
public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Setting", "dbo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();

        builder.HasMany(x => x.SettingValues).WithOne(x => x.Setting).HasForeignKey(x => x.SettingFk).IsRequired();
    }
}

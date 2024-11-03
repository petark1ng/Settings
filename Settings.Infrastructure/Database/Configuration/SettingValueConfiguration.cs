using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Settings.Entities;

namespace Settings.Infrastructure.Database.Configuration;
internal class SettingValueConfiguration : IEntityTypeConfiguration<SettingValue>
{
    public void Configure(EntityTypeBuilder<SettingValue> builder)
    {
        builder.ToTable("SettingValue", "dbo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value).HasColumnName("Value").IsRequired();
        builder.Property(x => x.ValidFrom).HasColumnName("ValidFrom").IsRequired();
        builder.Property(x => x.ValidTo).HasColumnName("ValidTo").IsRequired(false);

        builder.Property(x => x.SettingFk).HasColumnName("SettingFk").IsRequired();

        builder.HasOne(x => x.Setting).WithMany(x => x.SettingValues).HasForeignKey(x => x.SettingFk).IsRequired();
    }
}

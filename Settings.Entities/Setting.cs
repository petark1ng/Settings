namespace Settings.Entities;

public class Setting : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<SettingValue> SettingValues { get; set; } = [];
}

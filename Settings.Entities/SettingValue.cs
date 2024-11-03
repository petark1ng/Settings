namespace Settings.Entities;
public class SettingValue : BaseEntity
{
    public int SettingFk { get; set; }

    public decimal Value { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public Setting Setting { get; set; }
}

namespace Settings.Contracts.Requests;
public class InsertSettingValueRequest
{
    public required DateTime ValidFrom { get; set; }

    public required decimal Value { get; set; }
}

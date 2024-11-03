using System.Text.Json.Serialization;

namespace Settings.Contracts.Responses;
public class SettingDto
{
    public int Id { get; set; }

    public int SettingValueId { get; set; }

    public required string SettingName { get; set; }

    public required DateTime ValidFrom { get; set; }

    public required decimal Value { get; set; }

    [JsonIgnore]
    public DateTime? ValidTo { get; set; }
}

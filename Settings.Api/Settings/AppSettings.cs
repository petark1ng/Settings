using Settings.Contracts.Settings;

namespace Settings.Api.Settings;

public class AppSettings : IAppSettings
{
    public const string Key = "ConnectionStrings";

    public required string DatabaseConnectionString { get; init; }
}
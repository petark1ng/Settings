using Settings.Contracts.Requests;

namespace Settings.Contracts.Interfaces.Services;
public interface ISettingsService
{
    Task InsertSettingAsync(InsertSettingRequest request);

    Task InsertSettingValueAsync(int settingId, InsertSettingValueRequest request);

    Task UpdateSettingValueAsync(UpdateSettingValueRequest request, int settingId, int settingValueId);

    Task DeleteSettingValueAsync(int settingId, int settingValueId);
}

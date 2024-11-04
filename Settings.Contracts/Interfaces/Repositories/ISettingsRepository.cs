using Settings.Contracts.Responses;
using Settings.Entities;

namespace Settings.Contracts.Interfaces.Repositories;
public interface ISettingsRepository
{
    void InsertSetting(Setting setting);

    void DeleteSettingValue(SettingValue setting);

    Task<IReadOnlyList<SettingDto>> GetSettingsAsync();

    Task<SettingValue> GetSettingValueAsync(int settingId, int settingValueId);

    Task<Setting> GetSettingAsync(int settingId);

    Task<bool> IsSettingNameAlreadyTakenAsync(string settingName);

    Task<SettingDto> GetSettingDtoAsync(int settingId, DateTime? validFrom);
}

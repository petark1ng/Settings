using Microsoft.EntityFrameworkCore;
using Settings.Contracts.Exceptions;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Responses;
using Settings.Entities;
using Settings.Infrastructure.Database.Context;

namespace Settings.Infrastructure.Repositories;
public class SettingsRepository : RepositoryBase, ISettingsRepository
{
    public SettingsRepository(ISettingsDbContext settingsDbContext) : base(settingsDbContext)
    {
    }

    public void InsertSetting(Setting setting)
    {
        Insert(setting);
    }

    public void DeleteSettingValue(SettingValue settingValue)
    {
        Delete(settingValue);
    }

    public async Task<IReadOnlyList<SettingDto>> GetSettingsAsync()
    {
        return await (from dbSetting in AllWithoutTracking<Setting>()
                      join dbSettingValues in AllWithoutTracking<SettingValue>()
                          on dbSetting.Id equals dbSettingValues.SettingFk
                      select new SettingDto
                      {
                          Id = dbSetting.Id,
                          SettingValueId = dbSettingValues.Id,
                          SettingName = dbSetting.Name,
                          ValidFrom = dbSettingValues.ValidFrom,
                          Value = dbSettingValues.Value
                      }).ToArrayAsync();
    }

    public async Task<SettingValue> GetSettingValueAsync(int settingId, int settingValueId)
    {
        SettingValue? dbSettingValue = await All<SettingValue>().Where(x => x.Id == settingValueId &&
                                                                            x.SettingFk == settingId)
                                                                .SingleOrDefaultAsync();

        if (dbSettingValue is null)
        {
            throw new NotFoundException($"Setting value with id {settingValueId} for setting with id {settingId} was not found");
        }

        return dbSettingValue;
    }

    public async Task<Setting> GetSettingAsync(int settingId)
    {
        Setting? dbSetting = await All<Setting>().Include(x => x.SettingValues)
                                                 .SingleOrDefaultAsync(x => x.Id == settingId);

        if (dbSetting is null)
        {
            throw new NotFoundException($"Setting with id {settingId} was not found");
        }

        return dbSetting;
    }

    public async Task<bool> IsSettingNameAlreadyTaken(string settingName)
    {
        return await AllWithoutTracking<Setting>().AnyAsync(x => x.Name.ToLower() == settingName.ToLower());
    }

    public async Task<SettingDto> GetSettingDto(int settingId, DateTime? validFrom)
    {
        IQueryable<SettingDto> settingsDtos = from dbSetting in AllWithoutTracking<Setting>().Where(x => x.Id == settingId)
                                              join dbSettingValues in AllWithoutTracking<SettingValue>()
                                                  on dbSetting.Id equals dbSettingValues.SettingFk
                                              select new SettingDto
                                              {
                                                  Id = dbSetting.Id,
                                                  SettingValueId = dbSettingValues.Id,
                                                  SettingName = dbSetting.Name,
                                                  ValidFrom = dbSettingValues.ValidFrom,
                                                  ValidTo = dbSettingValues.ValidTo,
                                                  Value = dbSettingValues.Value
                                              };

        if (validFrom.HasValue)
        {
            return await settingsDtos.SingleOrDefaultAsync(x => (x.ValidFrom <= validFrom.Value && x.ValidTo.HasValue && validFrom.Value <= x.ValidTo) ||
                                                                (x.ValidFrom <= validFrom && !x.ValidTo.HasValue))
                ?? throw new NotFoundException("No setting values for the provided period");
        }

        return await settingsDtos.OrderByDescending(x => x.ValidFrom)
                                 .FirstOrDefaultAsync()
           ?? throw new NotFoundException("No settings values found.");
    }
}

using Settings.Contracts.Exceptions;
using Settings.Contracts.Interfaces;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Interfaces.Services;
using Settings.Contracts.Requests;
using Settings.Entities;

namespace Settings.Services;

public class SettingsService : ISettingsService
{
    private const int MINUS_ONE = -1;

    private readonly ISettingsRepository _settingsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SettingsService(ISettingsRepository settingsRepository,
                           IUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task InsertSettingAsync(InsertSettingRequest request)
    {
        bool isSettingNameTaken = await _settingsRepository.IsSettingNameAlreadyTakenAsync(settingName: request.Name);

        if (isSettingNameTaken)
        {
            throw new AlreadyExistsException("Setting with that name already exists");
        }

        Setting newSetting = new Setting
        {
            Name = request.Name
        };

        _settingsRepository.InsertSetting(newSetting);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task InsertSettingValueAsync(int settingId, InsertSettingValueRequest request)
    {
        Setting dbSetting = await _settingsRepository.GetSettingAsync(settingId: settingId);

        bool isSettingValuePeriodOverlapping = dbSetting.SettingValues.Any(x => x.ValidFrom <= request.ValidFrom &&
                                                                                x.ValidTo.HasValue &&
                                                                                x.ValidTo >= request.ValidFrom);

        if (isSettingValuePeriodOverlapping)
        {
            throw new NotAllowedException("There is a setting value already declared for that period");
        }

        var newSettingValue = new SettingValue
        {
            ValidFrom = request.ValidFrom,
            Value = request.Value
        };

        SettingValue? latestSettingValue = dbSetting.SettingValues.SingleOrDefault(x => !x.ValidTo.HasValue);

        if (latestSettingValue is not null)
        {
            if (request.ValidFrom > latestSettingValue.ValidFrom)
            {
                latestSettingValue.ValidTo = GetAppropriateSettingValueValidToDate(request.ValidFrom);
            }
            else
            {
                SettingValue? firstSettingValue = dbSetting.SettingValues.MinBy(x => x.ValidFrom)
                                                                         ?? throw new NotFoundException("Setting with the lowest validity value not found");

                newSettingValue.ValidTo = GetAppropriateSettingValueValidToDate(firstSettingValue.ValidFrom);
            }
        }

        dbSetting.SettingValues.Add(newSettingValue);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateSettingValueAsync(UpdateSettingValueRequest request, int settingId, int settingValueId)
    {
        SettingValue dbSettingValue = await _settingsRepository.GetSettingValueAsync(settingId: settingId,
                                                                                     settingValueId: settingValueId);

        dbSettingValue.Value = request.Value;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSettingValueAsync(int settingId, int settingValueId)
    {
        Setting dbSetting = await _settingsRepository.GetSettingAsync(settingId: settingId);

        SettingValue? dbSettingValueToBeDeleted = dbSetting.SettingValues.SingleOrDefault(x => x.Id == settingValueId)
                                                                         ?? throw new NotFoundException($"The setting value with id: {settingValueId} for setting with id: {settingId} was not found");

        if (dbSettingValueToBeDeleted.ValidTo.HasValue)
        {
            SettingValue? previousSettingValue = GetPreviousSettingValue(dbSetting, dbSettingValueToBeDeleted);

            if (previousSettingValue is not null)
            {
                SettingValue? nextSettingValue = dbSetting.SettingValues.SingleOrDefault(x => x.ValidFrom.AddSeconds(MINUS_ONE) == dbSettingValueToBeDeleted.ValidTo.Value)
                                                                         ?? throw new NotFoundException($"Internal server error.");

                previousSettingValue.ValidTo = GetAppropriateSettingValueValidToDate(nextSettingValue.ValidFrom);
            }
        }
        else
        {
            SettingValue? previousSettingValue = GetPreviousSettingValue(dbSetting, dbSettingValueToBeDeleted);

            if (previousSettingValue is not null)
            {
                previousSettingValue.ValidTo = null;
            }
        }

        _settingsRepository.DeleteSettingValue(dbSettingValueToBeDeleted);

        await _unitOfWork.SaveChangesAsync();
    }

    private static SettingValue? GetPreviousSettingValue(Setting dbSetting, SettingValue dbSettingValueToBeDeleted)
    {
        return dbSetting.SettingValues.SingleOrDefault(x => x.ValidTo.HasValue &&
                                                            x.ValidTo.Value == dbSettingValueToBeDeleted.ValidFrom.AddSeconds(MINUS_ONE));
    }

    private static DateTime GetAppropriateSettingValueValidToDate(DateTime validFrom)
    {
        return validFrom.AddSeconds(MINUS_ONE);
    }
}

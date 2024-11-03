﻿using Settings.Contracts.Exceptions;
using Settings.Contracts.Interfaces;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Interfaces.Services;
using Settings.Contracts.Requests;
using Settings.Entities;

namespace Settings.Services;

public class SettingsService : ISettingsService
{
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
        bool isSettingNameTaken = await _settingsRepository.IsSettingNameAlreadyTaken(settingName: request.Name);

        if (isSettingNameTaken)
        {
            throw new AlreadyExistsException("Setting with that name already exists"); // Test.
        }

        Setting newSetting = new Setting
        {
            Name = request.Name
        };

        _settingsRepository.InsertSetting(newSetting); // Test.

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task InsertSettingValueAsync(int settingId, InsertSettingValueRequest request)
    {
        Setting dbSetting = await _settingsRepository.GetSettingAsync(settingId: settingId);

        IReadOnlyList<SettingValue> dbSettingValues = await _settingsRepository.GetSettingsValuesAsync(settingId: settingId); // da gi spojam ovoj i gorniot vo eden.

        bool isSettingValuePeriodOverlapping = dbSettingValues.Any(x => x.ValidFrom <= request.ValidFrom &&
                                                                        x.ValidTo.HasValue &&
                                                                        x.ValidTo >= request.ValidFrom); // maybe first check if no elements, than avoid all the code under.
                                                                                                         // compare x.ValidTo with logic from repo.

        if (isSettingValuePeriodOverlapping)
        {
            throw new NotAllowedException("There is a setting value already declared for that period"); // Test.
        }

        var newSettingValue = new SettingValue
        {
            ValidFrom = request.ValidFrom,
            Value = request.Value
        };

        SettingValue? latestSettingValue = dbSettingValues.SingleOrDefault(x => !x.ValidTo.HasValue);

        if (latestSettingValue is not null)
        {
            if (request.ValidFrom > latestSettingValue.ValidFrom)
            {
                latestSettingValue.ValidTo = request.ValidFrom.AddSeconds(-1);
            }
            else
            {
                SettingValue? firstSettingValue = dbSettingValues.MinBy(x => x.ValidFrom);

                if (firstSettingValue is null)
                {
                    throw new NotFoundException("Setting with the lowest validity value not found"); // Test.
                }

                newSettingValue.ValidTo = firstSettingValue.ValidFrom.AddSeconds(-1);
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
        SettingValue dbSettingValue = await _settingsRepository.GetSettingValueAsync(settingId: settingId,
                                                                                     settingValueId: settingValueId);

        _settingsRepository.DeleteSettingValue(dbSettingValue);

        await _unitOfWork.SaveChangesAsync();
    }
}
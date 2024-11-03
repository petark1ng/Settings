using Microsoft.AspNetCore.Mvc;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Interfaces.Services;
using Settings.Contracts.Requests;
using Settings.Contracts.Responses;

namespace Settings.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsRepository settingsRepository,
                              ISettingsService settingsService)
    {
        _settingsRepository = settingsRepository;
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IEnumerable<SettingDto>> GetSettingsAsync()
    {
        return await _settingsRepository.GetSettingsAsync();
    }

    [HttpPost]
    [Route("")]
    public async Task InsertSettingAsync([FromBody] InsertSettingRequest request)
    {
        await _settingsService.InsertSettingAsync(request: request);
    }

    [HttpGet]
    [Route("{settingId:int}")]
    public async Task<SettingDto> GetSettingValue([FromRoute] int settingId, [FromQuery] DateTime? validFrom)
    {
        return await _settingsRepository.GetSettingDto(settingId: settingId,
                                                       validFrom: validFrom);
    }

    [HttpPost]
    [Route("{settingId:int}")]
    public async Task InsertSettingValueAsync([FromRoute] int settingId, [FromBody] InsertSettingValueRequest request)
    {
        await _settingsService.InsertSettingValueAsync(settingId: settingId,
                                                       request: request);
    }

    [HttpPatch]
    [Route("{settingId:int}/settingsValues/{settingValueId:int}")]
    public async Task UpdateSettingValueAsync([FromRoute] int settingId, [FromRoute] int settingValueId, [FromBody] UpdateSettingValueRequest request)
    {
        await _settingsService.UpdateSettingValueAsync(request: request,
                                                       settingId: settingId,
                                                       settingValueId: settingValueId);
    }

    [HttpDelete]
    [Route("{settingId:int}/settingsValues/{settingValueId:int}")]
    public async Task DeleteSettingValueAsync([FromRoute] int settingId, [FromRoute] int settingValueId)
    {
        await _settingsService.DeleteSettingValueAsync(settingId: settingId,
                                                       settingValueId: settingValueId);
    }
}

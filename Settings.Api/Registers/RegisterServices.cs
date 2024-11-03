using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Settings.Api.Settings;
using Settings.Contracts.Interfaces;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Interfaces.Services;
using Settings.Contracts.Settings;
using Settings.Infrastructure.Database.Context;
using Settings.Infrastructure.Repositories;
using Settings.Infrastructure.UnitOfWork;
using Settings.Services;

namespace Settings.Api.Registers;

public static class RegisterServices
{
    public static IServiceCollection Register(this IServiceCollection services)
    {
        // Settings
        services.AddOptions<AppSettings>()
            .BindConfiguration(AppSettings.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

        // Services
        services.AddScoped<ISettingsService, SettingsService>();

        // Infrastructure
        services.AddScoped<ISettingsDbContext, SettingsDbContext>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<SettingsDbContext>((provider, options) =>
        {
            IAppSettings appSettings = provider.GetService<IAppSettings>()!;

            options.UseSqlServer(
                appSettings.DatabaseConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(15),
                    errorNumbersToAdd: null);
                });
        });

        return services;
    }
}

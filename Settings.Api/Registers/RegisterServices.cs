using Microsoft.EntityFrameworkCore;
using Settings.Contracts.Interfaces;
using Settings.Contracts.Interfaces.Repositories;
using Settings.Contracts.Interfaces.Services;
using Settings.Infrastructure.Database.Context;
using Settings.Infrastructure.Repositories;
using Settings.Infrastructure.UnitOfWork;
using Settings.Services;

namespace Settings.Api.Registers;

public static class RegisterServices
{
    public static IServiceCollection Register(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ISettingsService, SettingsService>();

        // Infrastructure
        services.AddScoped<ISettingsDbContext, SettingsDbContext>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<SettingsDbContext>((provider, options) =>
        {
            // IAppSettings appSettings = provider.GetService<IAppSettings>();

            options.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=SettingsDatabase;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True", // recheck this and use it from appsettings.
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

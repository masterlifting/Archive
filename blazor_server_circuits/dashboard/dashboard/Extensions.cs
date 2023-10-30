using System.Globalization;

using Dapr.Client;

using dashboard.Domains._Extra.AutoComplete.Services.Implementations;
using dashboard.Domains._Extra.AutoComplete.Services.Interfaces;
using dashboard.Domains._Extra.Documents.Services.Implementations;
using dashboard.Domains._Extra.Documents.Services.Interfaces;
using dashboard.Domains._Extra.FileUploader.Services.Implementations;
using dashboard.Domains._Extra.FileUploader.Services.Interfaces;
using dashboard.Domains._Extra.UserConfigurations.Services.Implementations;
using dashboard.Domains._Extra.UserConfigurations.Services.Interfaces;
using dashboard.Domains.Warehouse;
using dashboard.Domains.Workflows;
using dashboard.Services.Authorization;
using dashboard.Services.Circuits;
using dashboard.Services.Fluxor.Store.Web;

using Dashboard;

using Fluxor;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

using MudBlazor;
using MudBlazor.Services;

using Serilog;
using Serilog.Core;

namespace dashboard;

public static class Extensions
{
    public static string? Version { get; set; }
    public static readonly CultureInfo[] SupportedCultures = new CultureInfo[]
    {
        new("en-US"),
        new("de-DE"),
        new("nl-NL"),
        new("ru-RU")
    };

    public static string[] GetSupportedCulturesNames()
    {
        return SupportedCultures.Select(c => c.Name).ToArray();
    }

    public static void AddLogging(LoggingLevelSwitch levelSwitch)
    {
        // Setup logging before anything else.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console()
            .WriteTo.File("logs/bootstrap.log")
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithAssemblyInformationalVersion()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .CreateBootstrapLogger();

        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var informationalVersion = assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .FirstOrDefault();

        Log.ForContext<Program>()
            .Information("Assembly full name: {Fullname}", assembly.FullName);
        Log.ForContext<Program>()
            .Information("Location: {Location}", assembly.Location);

        Version = informationalVersion?.InformationalVersion ?? "N/A";

        Log.ForContext<Program>()
            .Information("Informational version: {InformationalVersion}", Version);
    }
    public static IServiceCollection AddDashboard(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddHttpContextAccessor();

        services.AddTransient<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
        services.AddTransient<UserAuthorization>();

        services.AddScoped<DashboardCircuit>();
        services.AddScoped<CircuitHandler, DashboardCircuitHandler>();

        services.AddSingleton(new DaprClientBuilder().Build());

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
            config.SnackbarConfiguration.VisibleStateDuration = 8000;
            config.SnackbarConfiguration.HideTransitionDuration = 300;
            config.SnackbarConfiguration.ShowTransitionDuration = 300;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        });

        services.AddFluxor(x =>
        {
            x.ScanAssemblies(typeof(Program).Assembly);
            x.WithLifetime(StoreLifetime.Singleton);
        });

        //TODO: MOCK SERVICES
        services
            .AddSingleton<UserCircuitsService>()
            .AddTransient<IUserConfiguration, DaprUserConfiguration>()
            .AddTransient<IDocumentsViewer, DaprDocumentsViewer>()
            .AddTransient<IFilesUploader, MockFilesUploader>()
            .AddTransient<IAutoCompleteSearch, DaprAutoCompleteSearch>()
            .AddWorkflows()
            .AddWarehouse();

        return services;
    }
}

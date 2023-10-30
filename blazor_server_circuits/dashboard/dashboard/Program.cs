using System.Reflection;

using dashboard;

using HealthChecks.ApplicationStatus.DependencyInjection;

using Lungmuss.Refractory.Library;
using Lungmuss.Refractory.Library.Extensions;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Prometheus;

using Serilog;
using Serilog.Core;

namespace Dashboard;

public class Program
{
    public static void Main(string[] args)
    {
        var levelSwitch = new LoggingLevelSwitch();
       
        Extensions.AddLogging(levelSwitch);

        #region dump environment variables into the log.

        Common.DumpEnvironmentVariables(Log.Logger);

        #endregion

        #region default logging

        Log.ForContext<Program>().Information("Running in container: {RunningInContainer}", EnvironmentVars.runningInContainer);
        Log.ForContext<Program>().Verbose("Starting program {@FullInfo}", Assembly.GetExecutingAssembly());
        Log.ForContext<Program>()
            .Information("{ImageRuntimeVersion}", Assembly.GetExecutingAssembly().ImageRuntimeVersion);
        Log.ForContext<Program>().Information("CreateHostBuilder {@Args}", args);
        Log.ForContext<Program>().Information("Build and run host");

        #endregion

        var builder = WebApplication.
            CreateBuilder(args);

        #region set up serilog

        builder.Services.AddLogging();

        builder
            .Host
            .UseSerilog((
                hostingContext
                , loggerConfiguration
            ) => loggerConfiguration.ReadFrom
                .Configuration(hostingContext.Configuration)
                .MinimumLevel.ControlledBy(levelSwitch));

        builder.Logging.AddSerilog();

        #endregion

        builder.Services.AddRazorPages();
        builder.Services.AddControllers().AddDapr();
        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

        builder.Services.AddDashboard();

        builder.Services.AddDaprActors(new List<string>());

        #region add health checks

        // https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
        // https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

        var healthChecksBuilder = builder.Services.AddHealthChecks();

        // https://stackoverflow.com/questions/65858309/why-do-i-need-3-different-kind-of-probes-in-kubernetes-startupprobe-readinessp

        healthChecksBuilder
            .AddApplicationStatus("application_status", tags: new[]
            {
                "ready", "startup"
            }).ForwardToPrometheus()
            .AddProcessAllocatedMemoryHealthCheck(1000, "memory", tags: new[]
            {
                "live"
            }).ForwardToPrometheus();
        #endregion

        var app = builder.Build();
        
        app.UseStaticFiles();
        app.UseSerilogRequestLogging();

        app.UseCloudEvents();
        app.UseHttpLogging();

        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        
        var supportedCulturesNames = Extensions.GetSupportedCulturesNames();
        
        app.UseRequestLocalization(new RequestLocalizationOptions()
            .AddSupportedCultures(supportedCulturesNames)
            .AddSupportedUICultures(supportedCulturesNames));

        app.UseRouting();

        app.UseHttpMetrics();

        app.MapMetrics()
            .AllowAnonymous();

        app.MapActorsHandlers()
            .AllowAnonymous();

        // https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/publish-subscribe
        app.MapSubscribeHandler().AllowAnonymous(); // needed for Dapr pub/sub routing

        app.UseAuthorization();

        app.MapHealthChecks(EnvironmentVars.livez, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        }).AllowAnonymous();

        app.MapHealthChecks(EnvironmentVars.readyz, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        }).AllowAnonymous();

        app.MapHealthChecks(EnvironmentVars.startUpz, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("startup")
        }).AllowAnonymous();

        app.Run();
    }
}

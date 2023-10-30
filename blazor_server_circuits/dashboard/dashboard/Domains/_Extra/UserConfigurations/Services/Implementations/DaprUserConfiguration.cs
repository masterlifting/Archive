using Dapr.Client;

using dashboard.Domains._Extra.UserConfigurations.Models;
using dashboard.Domains._Extra.UserConfigurations.Services.Interfaces;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using MudBlazor;

namespace dashboard.Domains._Extra.UserConfigurations.Services.Implementations;

public sealed class DaprUserConfiguration : IUserConfiguration
{
    private readonly DaprClient _daprClient;
    private readonly IDispatcher _dispatcher;
    private const string KeyPrefix = "user_configuration_";

    public DaprUserConfiguration(DaprClient daprClient, IDispatcher dispatcher)
    {
        _daprClient = daprClient;
        _dispatcher = dispatcher;
    }

    public Task<UserConfiguration> Get(string userName, CancellationToken cToken = default)
    {
        try
        {
            return _daprClient.GetStateAsync<UserConfiguration>("statestore", $"{KeyPrefix}{userName}", cancellationToken: cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load user configuration", Severity.Error)));
            return Task.FromResult(new UserConfiguration());
        }
    }

    public Task Set(string userName, UserConfiguration configuration, CancellationToken cToken = default)
    {
        try
        {
            return _daprClient.SaveStateAsync("statestore", $"{KeyPrefix}{userName}", configuration, cancellationToken: cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't save user configuration", Severity.Error)));
            return Task.CompletedTask;
        }
    }
}

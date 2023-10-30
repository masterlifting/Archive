using Dapr.Client;

using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using MudBlazor;

namespace dashboard.Domains.Warehouse.Inbound.Services.Implementations;

public sealed class DaprWarehouseInboundState : IWarehouseInboundState
{
    private readonly DaprClient _daprClient;
    private readonly IWarehouseInboundData _inboundData;
    private readonly IDispatcher _dispatcher;
    private const string KeyPrefix = "inbound_create_";

    public DaprWarehouseInboundState(DaprClient daprClient, IWarehouseInboundData inboundData, IDispatcher dispatcher)
    {
        _daprClient = daprClient;
        _inboundData = inboundData;
        _dispatcher = dispatcher;
    }

    public async Task<InboundWorkflowCreate> Get(string userName, CancellationToken cToken = default)
    {
        try
        {
            var state = await _daprClient.GetStateAsync<InboundWorkflowCreate>("statestore",$"{KeyPrefix}{userName}", cancellationToken: cToken);

            return state ?? await _inboundData.GetNewCreationModel(cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load last inbound state", Severity.Error)));
            return await _inboundData.GetNewCreationModel(cToken);
        }
    }

    public Task Set(string userName, InboundWorkflowCreate workflow, CancellationToken cToken = default)
    {
        try
        {
            return _daprClient.SaveStateAsync("statestore", $"{KeyPrefix}{userName}", workflow, cancellationToken: cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't save inbound state", Severity.Error)));
            return Task.CompletedTask;
        }
    }

    public Task Clear(string userName, CancellationToken cToken = default)
    {
        try
        {
            return _daprClient.DeleteStateAsync("statestore", $"{KeyPrefix}{userName}", cancellationToken: cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't clear inbound state", Severity.Error)));
            return Task.CompletedTask;
        }
    }
}

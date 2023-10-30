using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

namespace dashboard.Domains.Warehouse.Inbound.Services.Implementations;

public sealed class MockWarehouseInboundState : IWarehouseInboundState
{
    private static InboundWorkflowCreate? State;
    private readonly IWarehouseInboundData _inboundData;
    public MockWarehouseInboundState(IWarehouseInboundData inboundData) => _inboundData = inboundData;

    public Task Clear(string userName, CancellationToken cToken = default)
    {
        State = null;
        return Task.CompletedTask;
    }

    public async Task<InboundWorkflowCreate> Get(string userName, CancellationToken cToken = default)
    {
        return State ?? await _inboundData.GetNewCreationModel(cToken);
    }

    public Task Set(string userName, InboundWorkflowCreate workflow, CancellationToken cToken = default)
    {
        State = workflow;
        return Task.CompletedTask;
    }
}

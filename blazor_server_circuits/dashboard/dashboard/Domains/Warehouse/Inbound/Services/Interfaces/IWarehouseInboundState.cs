using dashboard.Domains.Warehouse.Inbound.Models;

namespace dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

public interface IWarehouseInboundState
{
    Task<InboundWorkflowCreate> Get(string userName, CancellationToken cToken = default);
    Task Set(string userName, InboundWorkflowCreate workflow, CancellationToken cToken = default);
    Task Clear(string userName, CancellationToken cToken = default);
}

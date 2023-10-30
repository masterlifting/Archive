using dashboard.Domains.Warehouse.Inbound.Models;

namespace dashboard.Domains.Warehouse.Inbound.Store;

public sealed record WarehouseInboundUpdateAction(string UserName, InboundWorkflow[] Workflows);
public sealed record WarehouseInboundAddAction(string UserName, InboundWorkflow Workflow);
public sealed record WarehouseInboundRemoveAction(string UserName, string WorkflowId);

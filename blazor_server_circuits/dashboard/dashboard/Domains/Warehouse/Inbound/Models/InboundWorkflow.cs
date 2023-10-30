namespace dashboard.Domains.Warehouse.Inbound.Models;

public sealed record InboundWorkflow(string Id, DateTime CreatedOn, string OrderNumber, string Description);

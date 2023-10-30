using dashboard.Domains.Warehouse.Inbound.Models;

namespace dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

public interface IWarehouseInboundData
{
    Task<InboundWorkflow[]> GetStarted(string userName, CancellationToken cToken = default);
    Task<InboundWorkflowDetails?> GetDetails(string workflowId, CancellationToken cToken = default);
    Task<InboundWorkflowCategory[]> GetCategories(CancellationToken cToken = default);
    Task<InboundWorkflowArticle[]> GetArticles(CancellationToken cToken = default);
    
    Task<InboundWorkflowCreate> GetNewCreationModel(CancellationToken cToken = default);
    Task Create(InboundWorkflowCreate workflow, CancellationToken cToken = default);

    string? TryValidateWeight(string value, out double result);
}

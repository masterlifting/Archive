using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

namespace dashboard.Domains.Warehouse.Inbound.Services.Implementations;

internal sealed class MockWarehouseInboundData : IWarehouseInboundData
{
    private static List<InboundWorkflow> InboundProcesses = new();
    
    public Task<InboundWorkflow[]> GetStarted(string userName, CancellationToken cToken = default)
    {
        return Task.FromResult(InboundProcesses.ToArray());
    }
    public Task<InboundWorkflowDetails?> GetDetails(string workflowId, CancellationToken cToken = default)
    {
        var process = InboundProcesses.FirstOrDefault(x => x.Id == workflowId);

        return process is null
            ? null
            : Task.FromResult(new InboundWorkflowDetails(new(new(), new())));
    }
    public Task<InboundWorkflowCategory[]> GetCategories(CancellationToken cToken = default)
    {
        return Task.FromResult(new[]
        {
            new InboundWorkflowCategory(1, "Category 1"),
            new InboundWorkflowCategory(2, "Category 2"),
            new InboundWorkflowCategory(3, "Category 3"),
        });
    }
    public Task<InboundWorkflowArticle[]> GetArticles(CancellationToken cToken = default)
    {
        return Task.FromResult(Enumerable.Range(1, 3000).Select(x => 
            new InboundWorkflowArticle(x, $"Name {x}", $"ArticleId {x}", $"Supplier Id {x}")).ToArray());
    }
    
    public Task<InboundWorkflowCreate> GetNewCreationModel(CancellationToken cToken = default)
    {
        return Task.FromResult(new InboundWorkflowCreate());
    }
    public Task Create(InboundWorkflowCreate workflow, CancellationToken cToken = default)
    {
        InboundProcesses.Add(new InboundWorkflow(Guid.NewGuid().ToString(), DateTime.UtcNow, "OrderNumber", "Article supplier's description"));
        return Task.CompletedTask;
    }

    public string? TryValidateWeight(string value, out double result)
    {
        result = 1;
        return null;
    }
}

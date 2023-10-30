using Dapr.Client;

using dashboard.Domains.Workflows.Models;
using dashboard.Domains.Workflows.Services.Interfaces;

namespace dashboard.Domains.Workflows.Services.Implementations;

public sealed class MockWorkflowsData : IWorkflowsData
{
    public Task<Workflow[]> GetUserRolesWorkflows(string userName, CancellationToken cToken = default)
    {
        var createdBy = DateTime.UtcNow.ToString();

        var workflows = Array.Empty<Workflow>();

        return Task.FromResult(workflows);
    }
    public Task<Workflow[]> GetUserWorkflows(string userName, CancellationToken cToken = default)
    {
        var createdBy = DateTime.UtcNow.ToString();

        var workflows = Array.Empty<Workflow>();

        return Task.FromResult(workflows);
    }
    public Task<WorkflowDetails> GetWorkflowDetails(string workflowId, CancellationToken cToken = default)
    {
        return Task.FromResult(new WorkflowDetails(new()
        {
            CreatedAt = DateTime.UtcNow,
            InstanceId = nameof(GetWorkflowResponse.InstanceId),
            RuntimeStatus = WorkflowRuntimeStatus.Running,
            LastUpdatedAt = DateTime.UtcNow,
            FailureDetails = new WorkflowFailureDetails(nameof(GetWorkflowResponse.FailureDetails.ErrorMessage), nameof(GetWorkflowResponse.FailureDetails.StackTrace)),
            WorkflowComponentName = nameof(GetWorkflowResponse.WorkflowComponentName),
            WorkflowName = nameof(GetWorkflowResponse.WorkflowName),
            Properties = new Dictionary<string, string>()
        }, new()));
    }
    public Task<WorkflowMetadata> GetWorkflowMetadata(string workflowId, CancellationToken cToken = default)
    {
        return Task.FromResult(new WorkflowMetadata(
            nameof(WorkflowMetadataDto.Subsystem), 
            nameof(WorkflowMetadataDto.CreatedBy),
            DateTime.UtcNow,
            nameof(WorkflowMetadataDto.Type),
            nameof(WorkflowMetadataDto.OrderId),
            nameof(WorkflowMetadataDto.ArticleSupplierDescription),
            nameof(WorkflowMetadataDto.Status)));
    }
}

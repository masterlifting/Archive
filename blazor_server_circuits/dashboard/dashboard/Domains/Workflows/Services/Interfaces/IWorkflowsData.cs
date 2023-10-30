using dashboard.Domains.Workflows.Models;

namespace dashboard.Domains.Workflows.Services.Interfaces;

public interface IWorkflowsData
{
    Task<Workflow[]> GetUserWorkflows(string userName, CancellationToken cToken = default);
    Task<Workflow[]> GetUserRolesWorkflows(string userName, CancellationToken cToken = default);
    Task<WorkflowDetails> GetWorkflowDetails(string workflowId, CancellationToken cToken = default);
    Task<WorkflowMetadata> GetWorkflowMetadata(string workflowId, CancellationToken cToken = default);
}

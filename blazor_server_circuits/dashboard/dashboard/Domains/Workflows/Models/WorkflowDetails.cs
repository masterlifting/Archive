using Dapr.Client;

using Lungmuss.Refractory.Library.Models.Workflows;

namespace dashboard.Domains.Workflows.Models;

public sealed record WorkflowDetails(GetWorkflowResponse Data, List<WorkflowDocument> Documents);

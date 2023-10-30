using dashboard.Domains.Workflows.Models;

using Lungmuss.Refractory.Library.Models.Workflows;

namespace dashboard.Domains.Workflows.Store;

public sealed record WorkflowUpdateAction(string UserName, Workflow Workflow);
public sealed record WorkflowsUpdateAction(string UserName, Workflow[] Workflows);
public sealed record WorkflowsReceiveNotificationAction(NotificationMessageV1 Notification);

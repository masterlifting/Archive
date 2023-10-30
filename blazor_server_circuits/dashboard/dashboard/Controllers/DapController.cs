using Dapr;

using dashboard.Domains.Workflows.Store;

using Fluxor;

using Lungmuss.Refractory.Library.Models.Workflows;

using Microsoft.AspNetCore.Mvc;

namespace dashboard.Controllers;

public sealed class DapController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public DapController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    
    [HttpPost("/WorkflowNotification"), Topic("dapr-pubsub", "WorkflowNotification")]
    public void ReceiveWorkflowNotification([FromBody] NotificationMessageV1 data) =>
        _dispatcher.Dispatch(new WorkflowsReceiveNotificationAction(data));
}

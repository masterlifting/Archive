using System.Globalization;

using dashboard.Domains._Extra.UserConfigurations.Store;
using dashboard.Domains.Warehouse.Inbound.Components;
using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Store;
using dashboard.Domains.Workflows.Models;
using dashboard.Domains.Workflows.Services.Interfaces;
using dashboard.Services.Fluxor.Store.Web;
using dashboard.Services.Fluxor.Store.Web.Models;

using Fluxor;

using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains.Workflows.Store;

public sealed class WorkflowsEffects
{
    private readonly IServiceScopeFactory _scopeFactory;
    public WorkflowsEffects(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    [EffectMethod]
    public async Task HandleNotification(WorkflowsReceiveNotificationAction action, IDispatcher dispatcher)
    {
        using var scope = _scopeFactory.CreateAsyncScope();

        var dataService = scope.ServiceProvider.GetRequiredService<IWorkflowsData>();
        var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<App>>();
        var userConfigurationState = scope.ServiceProvider.GetRequiredService<IState<UserConfigurationState>>();

        var userConfiguration = userConfigurationState.Value.Configuration;

        CultureInfo.CurrentCulture = userConfiguration.Culture;
        CultureInfo.CurrentUICulture = userConfiguration.Culture;

        var severity = action.Notification.EventType switch
        {
            "WorkflowStarted" => Severity.Info,
            "WorkflowCompleted" => Severity.Success,
            "WorkflowError" => Severity.Error,
            _ => Severity.Normal
        };

        var text = $@"
        <ul>
            <li>{localizer["Workflow type"]}: {action.Notification.WorkflowType}</li>
            <li>{localizer["Event type"]}: {action.Notification.EventType}</li>
            <li>{localizer["Description"]}: {action.Notification.Description}</li>
        </ul>";


        dispatcher.Dispatch(new SnackbarShowAction(action.Notification.UserName, new(text, severity)));

        var metadata = await dataService.GetWorkflowMetadata(action.Notification.WorkflowId);

        if (metadata is not null)
        {
            var workflow = new Workflow(action.Notification.WorkflowId, metadata);
            
            dispatcher.Dispatch(new WorkflowUpdateAction(action.Notification.UserName, workflow));

            if (action.Notification.EventType == "WorkflowStarted")
            {
                var startedWorkflow = new InboundWorkflow(action.Notification.WorkflowId, metadata.CreatedOn, metadata.OrderId, metadata.ArticleSupplierDescription);
                dispatcher.Dispatch(new WarehouseInboundAddAction(action.Notification.UserName, startedWorkflow));
            }
            else
            {
                dispatcher.Dispatch(new WarehouseInboundRemoveAction(action.Notification.UserName, action.Notification.WorkflowId));
            }

        }
        
        dispatcher.Dispatch(new StateHasChangedAction(action.Notification.UserName));

        if (!string.IsNullOrWhiteSpace(action.Notification.EventSubType)
            && action.Notification.EventSubType.Contains("Check", StringComparison.OrdinalIgnoreCase))
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = false,
                DisableBackdropClick = true,
                MaxWidth = MaxWidth.Medium,

            };
            var parameters = new DialogParameters<InboundConfirmation> {

                { nameof(InboundConfirmation.Content), action.Notification.Description },
                { nameof(InboundConfirmation.EventSubType), action.Notification.EventSubType },
                { nameof(InboundConfirmation.WorkflowId), action.Notification.WorkflowId }
            };

            var dialog = new DialogComponentModel(typeof(InboundConfirmation), action.Notification.WorkflowType, parameters, options);

            dispatcher.Dispatch(new DialogShowAction(action.Notification.UserName, dialog));
        }
    }
}

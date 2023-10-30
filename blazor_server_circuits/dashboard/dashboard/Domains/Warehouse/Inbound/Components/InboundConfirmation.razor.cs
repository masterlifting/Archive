using Dapr.Client;

using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Lungmuss.Refractory.Library.Extensions;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace dashboard.Domains.Warehouse.Inbound.Components
{
    public partial class InboundConfirmation : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

        [Parameter] public string Content { get; set; } = default!;
        [Parameter] public string EventSubType { get; set; } = default!;
        [Parameter] public string WorkflowId { get; set; } = default!;

        [Inject] DaprClient DaprClient { get; set; } = default!;
        [Inject] UserAuthorization Authorization { get; set; } = default!;
        [Inject] IDispatcher Dispatcher { get; set; } = null!;

        async Task Yes()
        {
            try
            {
                var userToken = await Authorization.GetUserToken();

                _ = await DaprClient.InvokeDaprRequest<bool, string>(
                   HttpMethod.Post,
                   WorkflowId.GetWorkflowAppId(),
                   $"v1/workflow/{WorkflowId}/{EventSubType}",
                   true,
                   default,
                   userToken);

                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception)
            {
                var userName = await Authorization.GetUserName();
                Dispatcher.Dispatch(new SnackbarShowAction(userName, new("Oops, something went wrong.", Severity.Error)));
            }
        }

        async Task No()
        {
            try
            {
                var userToken = await Authorization.GetUserToken();
                
                _ = await DaprClient.InvokeDaprRequest<bool, string>(
                   HttpMethod.Post,
                   WorkflowId.GetWorkflowAppId(),
                   $"v1/workflow/{WorkflowId}/{EventSubType}",
                   false,
                   default,
                   userToken);

                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception)
            {
                var userName = await Authorization.GetUserName();
                Dispatcher.Dispatch(new SnackbarShowAction(userName, new("Oops, something went wrong.", Severity.Error)));
            }
        }
    }
}

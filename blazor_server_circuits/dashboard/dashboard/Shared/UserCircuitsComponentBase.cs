using dashboard.Services.Authorization;
using dashboard.Services.Circuits;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace dashboard.Shared;

public abstract class UserCircuitsComponentBase : ComponentBase
{
    [Inject] public IDispatcher Dispatcher { get; set; } = null!;
    
    [Inject] public DashboardCircuit Circuit { get; set; } = default!;
    [Inject] public UserAuthorization Authorization { get; set; } = null!;
    
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService Dialog { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var userName = await Authorization.GetUserName();

            Dispatcher.Dispatch(new UserCircuitsServicesCreateAction(userName, Circuit.Current, new()
            {
                Snackbar = Snackbar,
                Dialog = Dialog,
                InvokeAsync = InvokeAsync,
                StateHasChanged = StateHasChanged,
            }));
        }
    }
}

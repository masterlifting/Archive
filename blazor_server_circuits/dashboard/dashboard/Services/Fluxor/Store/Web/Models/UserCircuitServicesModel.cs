using MudBlazor;

namespace dashboard.Services.Fluxor.Store.Web.Models;

public sealed record UserCircuitServicesModel
{
    public ISnackbar Snackbar { get; init; } = default!;
    public IDialogService Dialog { get; init; } = default!;
    public Func<Action, Task> InvokeAsync { get; init; } = default!;
    public Action StateHasChanged { get; set; } = default!;
}

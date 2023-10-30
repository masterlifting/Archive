using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace dashboard.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] IStringLocalizer<App> StringLocalizer { get; set; } = default!;

    private string _userName;

    override protected async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        _userName = authState.User?.Identity?.Name ?? StringLocalizer["You are unauthorized"];
    }
}

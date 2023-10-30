using dashboard.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace dashboard.Domains.Warehouse._Index.Pages;

[Authorize, Route("/warehouse")]
public partial class Warehouse : UserCircuitsComponentBase
{
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
}

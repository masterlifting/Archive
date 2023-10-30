using dashboard.Shared;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace dashboard.Pages;

[Route("/")]
public partial class Home : UserCircuitsComponentBase
{
    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
}

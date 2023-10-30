using dashboard.Domains._Extra.UserConfigurations.Models;
using System.Globalization;

using dashboard.Domains._Extra.UserConfigurations.Services.Interfaces;
using dashboard.Domains._Extra.UserConfigurations.Store;
using dashboard.Services.Authorization;

using Fluxor;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace dashboard;

public partial class App
{
    [Inject] IDispatcher Dispatcher { get; set; } = null!;
    [Inject] UserAuthorization Authorization { get; set; } = default!;
    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] IUserConfiguration UserConfiguration { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await Authorization.UserIsAuthorized())
            {
                var userName = await Authorization.GetUserName();
                var userConfiguration = await UserConfiguration.Get(userName);

                userConfiguration ??= new UserConfiguration()
                {
                    UserInfo = new UserInfo()
                    {
                        UserName = userName,
                    },
                    Culture = CultureInfo.CurrentCulture
                };

                Dispatcher.Dispatch(new UserConfigurationSetAction(userConfiguration));
            }
        }
    }
}

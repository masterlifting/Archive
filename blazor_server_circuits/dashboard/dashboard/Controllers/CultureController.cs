using dashboard.Domains._Extra.UserConfigurations.Store;

using Fluxor;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace dashboard.Controllers
{
    [Route("[controller]/[action]")]
    public sealed class CultureController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        public CultureController(IDispatcher dispatcher) => _dispatcher = dispatcher;
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)));

                var cultureInfo = Extensions.SupportedCultures.FirstOrDefault(x => x.Name == culture);
                
                if (cultureInfo is not null) 
                {
                    _dispatcher.Dispatch(new UserConfigurationUpdateCultureAction(cultureInfo));
                }
            }

            return LocalRedirect(redirectUri);
        }
    }
}

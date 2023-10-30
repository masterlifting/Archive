using dashboard.Domains._Extra.FileUploader.Services.Interfaces;
using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains._Extra.FileUploader.Services.Implementations;

public sealed class MockFilesUploader : IFilesUploader
{
    private readonly IStringLocalizer<App> _localizer;
    private readonly IDispatcher _dispatcher;
    private readonly UserAuthorization _authorization;

    public MockFilesUploader(IStringLocalizer<App> localizer, IDispatcher dispatcher, UserAuthorization authorization)
    {
        _localizer = localizer;
        _dispatcher = dispatcher;
        _authorization = authorization;
    }
    public Task UploadFile(IBrowserFile file)
    {
        var userName = _authorization.GetUserName().Result;
        _dispatcher.Dispatch(new SnackbarShowAction(userName, new($"{file.Name} {_localizer["was uploaded"]}", Severity.Success)));
        return Task.CompletedTask;
    }
}

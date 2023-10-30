using System.Net.Http.Headers;

using Dapr.Client;

using dashboard.Domains._Extra.FileUploader.Services.Interfaces;
using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Lungmuss.Refractory.Library.Extensions;

using Lungmuss.Refractory.Library.Models.Blob;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains._Extra.FileUploader.Services.Implementations;

public sealed class DaprFilesUploader : IFilesUploader
{
    private readonly DaprClient _daprClient;
    private readonly IDispatcher _dispatcher;
    private readonly UserAuthorization _authorization;
    private readonly IStringLocalizer<App> _localizer;

    public DaprFilesUploader(DaprClient daprClient, IDispatcher dispatcher, UserAuthorization authorization, IStringLocalizer<App> localizer)
    {
        _daprClient = daprClient;
        _dispatcher = dispatcher;
        _authorization = authorization;
        _localizer = localizer;
    }
    public async Task UploadFile(IBrowserFile file)
    {
        var userToken = await _authorization.GetUserToken();

        using var stream = file.OpenReadStream();

        using var streamContent = new StreamContent(stream);
        using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync(default));

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

        using var form = new MultipartFormDataContent
        {
            { fileContent, "blobFile", "merge.pdf" },
            { new StringContent("{ \"reference\": \"test_reference\", \"mime_type\" : \"test_mime_type\" }\r\n"), "payload" }
        };

        var (error, response) = await _daprClient.InvokeDaprRequest<BlobDocumentResponse>(
            HttpMethod.Post, 
            "blob",
            "v1/blob", 
            form, 
            default, userToken);

        var userName = await _authorization.GetUserName();

        if(!string.IsNullOrWhiteSpace(error))
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new(error, Severity.Error)));
        }
        else
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new($"{file.Name} {_localizer["was uploaded"]}", Severity.Success)));
        }
    }
}

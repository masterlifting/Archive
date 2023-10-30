using Dapr.Client;

using dashboard.Domains._Extra.Documents.Models;
using dashboard.Domains._Extra.Documents.Services.Interfaces;
using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Lungmuss.Refractory.Library.Extensions;

using Microsoft.JSInterop;

using MudBlazor;

using static dashboard.Domains.Constants.Extra;

namespace dashboard.Domains._Extra.Documents.Services.Implementations;

public class DaprDocumentsViewer : IDocumentsViewer
{
    private readonly IJSRuntime _jSRuntime;
    private readonly DaprClient _daprClient;
    private readonly UserAuthorization _authorization;
    private readonly IDispatcher _dispatcher;

    public DaprDocumentsViewer(IJSRuntime jSRuntime, DaprClient daprClient, UserAuthorization authorization, IDispatcher dispatcher)
    {
        _jSRuntime = jSRuntime;
        _daprClient = daprClient;
        _authorization = authorization;
        _dispatcher = dispatcher;
    }
    public Task<DashboardDocument[]> GetUserDocuments(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<DashboardDocument[]> GetUserDocuments(SupportedDocumentTypes type, CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<DashboardDocument> GetUserDocument(SupportedDocumentTypes type, string id, CancellationToken cToken = default)
    {
        return Task.FromResult(new DashboardDocument(id, type, "Document name", "Document description"));
    }

    public async Task OpenInNewTab(DashboardDocument document, CancellationToken cToken = default)
    {
        if (document.Type == SupportedDocumentTypes.Pdf)
        {
            try
            {
                var userToken = await _authorization.GetUserToken();

                var (Error, Stream) = await _daprClient.InvokeDaprRequestWithStreamResponse(
                    HttpMethod.Get, 
                    "blob", 
                    $"v1/blob/pdf/view/{document.Id}",
                    cToken, 
                    userToken);

                if (Error is not null)
                    throw new Exception(Error);

                if (Stream is null)
                    throw new Exception("Can't open document");

                using var contentStreamReference = new DotNetStreamReference(Stream);
                await _jSRuntime.InvokeVoidAsync("InternalFunctions.OpenPdfInNewTabWithStream", contentStreamReference);
            }
            catch (Exception)
            {
                var userName = await _authorization.GetUserName(cToken);
                _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't open document", Severity.Error)));
            }
        }
        else
            throw new NotSupportedException();
    }
}

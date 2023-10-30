using dashboard.Domains._Extra.Documents.Models;
using dashboard.Domains._Extra.Documents.Services.Interfaces;
using Microsoft.JSInterop;

using static dashboard.Domains.Constants.Extra;

namespace dashboard.Domains._Extra.Documents.Services.Implementations;

public sealed class MockDocumentsViewer : IDocumentsViewer
{
    private readonly IJSRuntime _jSRuntime;

    public MockDocumentsViewer(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
    }

    public Task<DashboardDocument[]> GetUserDocuments(CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<DashboardDocument> GetUserDocument(SupportedDocumentTypes type, string id, CancellationToken cToken = default)
    {
        if (type == SupportedDocumentTypes.Pdf)
        {
            var document = new DashboardDocument(id, type, "Description", $"/v1/blob/pdf/view/{id}");
            return Task.FromResult(document);
        }
        else
            throw new NotSupportedException();
    }
    public async Task<DashboardDocument[]> GetUserDocuments(SupportedDocumentTypes type, CancellationToken cToken = default)
    {
        var documents = new List<DashboardDocument>()
        {
            await GetUserDocument(type, "1", cToken),
            await GetUserDocument(type, "2", cToken),
            await GetUserDocument(type, "3", cToken),
        }
        .ToArray();

        return documents;
    }

    public async Task OpenInNewTab(DashboardDocument document, CancellationToken cToken)
    {
        if (document.Type == SupportedDocumentTypes.Pdf)
        {
            using var stream = File.OpenRead("wwwroot/1.pdf");
            using var contentStreamReference = new DotNetStreamReference(stream);
            await _jSRuntime.InvokeVoidAsync("InternalFunctions.OpenPdfInNewTabWithStream", contentStreamReference);
        }
        else
            throw new NotSupportedException();
    }
}

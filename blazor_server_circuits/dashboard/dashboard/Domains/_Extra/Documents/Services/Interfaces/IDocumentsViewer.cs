using dashboard.Domains._Extra.Documents.Models;

using static dashboard.Domains.Constants.Extra;

namespace dashboard.Domains._Extra.Documents.Services.Interfaces;

public interface IDocumentsViewer
{
    Task<DashboardDocument[]> GetUserDocuments(CancellationToken cToken = default);
    Task<DashboardDocument[]> GetUserDocuments(SupportedDocumentTypes type, CancellationToken cToken = default);
    Task<DashboardDocument> GetUserDocument(SupportedDocumentTypes type, string id, CancellationToken cToken = default);
    Task OpenInNewTab(DashboardDocument document, CancellationToken cToken = default);
}

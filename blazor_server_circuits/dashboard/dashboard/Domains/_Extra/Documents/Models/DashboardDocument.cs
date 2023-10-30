using static dashboard.Domains.Constants.Extra;

namespace dashboard.Domains._Extra.Documents.Models;
public sealed record DashboardDocument(string Id, SupportedDocumentTypes Type, string Description, string Url);

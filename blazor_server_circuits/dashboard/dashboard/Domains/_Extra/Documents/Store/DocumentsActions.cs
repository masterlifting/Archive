using dashboard.Domains._Extra.Documents.Models;

namespace dashboard.Domains._Extra.Documents.Store;

public sealed record DocumentsUpdateAction(string UserName, DashboardDocument[] Documents);

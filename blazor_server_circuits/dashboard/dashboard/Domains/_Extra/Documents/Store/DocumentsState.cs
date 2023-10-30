using dashboard.Domains._Extra.Documents.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains._Extra.Documents.Store;

[FeatureState]
public sealed record DocumentsState : FluxorUserMultiState<DashboardDocument>
{
    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    public DocumentsState(string userName, DashboardDocument[] documents) : base(userName, documents) { }

    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    private DocumentsState() : this(string.Empty, Array.Empty<DashboardDocument>()) { }
}

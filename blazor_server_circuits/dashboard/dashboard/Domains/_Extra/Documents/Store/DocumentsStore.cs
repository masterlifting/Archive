using dashboard.Domains._Extra.Documents.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains._Extra.Documents.Store;

[FeatureState]
public sealed record DocumentsStore : FluxorUserMultiStore<DocumentsState, DashboardDocument>
{
    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    public DocumentsStore(Dictionary<string, DocumentsState> data)
        : base(data) { }

    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    private DocumentsStore()
        : this(new Dictionary<string, DocumentsState>()) { }
}

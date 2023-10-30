using dashboard.Domains.Workflows.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains.Workflows.Store;

[FeatureState]
public sealed record WorkflowsStore : FluxorUserMultiStore<WorkflowsState, Workflow>
{
    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    public WorkflowsStore(Dictionary<string, WorkflowsState> data)
        : base(data) { }

    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    private WorkflowsStore()
        : this(new Dictionary<string, WorkflowsState>()) { }
}

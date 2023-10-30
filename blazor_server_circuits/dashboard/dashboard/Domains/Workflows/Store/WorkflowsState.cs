using dashboard.Domains.Workflows.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains.Workflows.Store;

[FeatureState]
public sealed record WorkflowsState : FluxorUserMultiState<Workflow>
{
    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    public WorkflowsState(string userName, Workflow[] workflows) : base(userName, workflows) { }

    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    private WorkflowsState() : this(string.Empty, Array.Empty<Workflow>()) { }
}

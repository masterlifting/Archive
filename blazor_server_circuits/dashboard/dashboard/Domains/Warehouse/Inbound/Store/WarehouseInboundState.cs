using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains.Warehouse.Inbound.Store;

[FeatureState]
public sealed record WarehouseInboundState : FluxorUserMultiState<InboundWorkflow>
{
    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    public WarehouseInboundState(string userName, InboundWorkflow[] workflows) : base(userName, workflows) { }

    /// <inheritdoc cref="FluxorUserMultiState{TModel}"/>
    private WarehouseInboundState() : this(string.Empty, Array.Empty<InboundWorkflow>()) { }
}

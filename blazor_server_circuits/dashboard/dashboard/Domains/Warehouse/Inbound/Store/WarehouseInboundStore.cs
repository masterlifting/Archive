using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Services.Fluxor;

using Fluxor;

namespace dashboard.Domains.Warehouse.Inbound.Store;

[FeatureState]
public sealed record WarehouseInboundStore : FluxorUserMultiStore<WarehouseInboundState, InboundWorkflow>
{
    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    public WarehouseInboundStore(Dictionary<string, WarehouseInboundState> data)
        : base(data) { }

    /// <inheritdoc cref="FluxorUserMultiStore{TState, TModel}"/>
    private WarehouseInboundStore()
        : this(new Dictionary<string, WarehouseInboundState>()) { }
}

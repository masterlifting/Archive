using dashboard.Domains.Warehouse.Inbound.Services.Implementations;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

namespace dashboard.Domains.Warehouse;

public static class Extensions
{
    public static IServiceCollection AddWarehouse(this IServiceCollection services)
    {
        services.AddTransient<IWarehouseInboundData, DaprWarehouseInboundData>();
        services.AddTransient<IWarehouseInboundState, DaprWarehouseInboundState>();

        return services;
    }
}

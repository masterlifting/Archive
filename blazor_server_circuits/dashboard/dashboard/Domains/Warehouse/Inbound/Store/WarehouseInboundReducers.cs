using dashboard.Domains.Warehouse.Inbound.Models;

using Fluxor;

namespace dashboard.Domains.Warehouse.Inbound.Store;

public class WarehouseInboundReducers
{
    [ReducerMethod]
    public static WarehouseInboundStore Update(WarehouseInboundStore oldStore, WarehouseInboundUpdateAction action)
    {
        var newStore = new WarehouseInboundStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var state = new WarehouseInboundState(action.UserName, action.Workflows);
            newStore.Data.Add(state.Id, state);
        }
        else
        {
            newStore.Data[action.UserName] = (newStore.Data[action.UserName] with
            {
                Models = action.Workflows
            });
        }

        return newStore;
    }

    [ReducerMethod]
    public static WarehouseInboundStore Add(WarehouseInboundStore oldStore, WarehouseInboundAddAction action)
    {
        var newStore = new WarehouseInboundStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var state = new WarehouseInboundState(action.UserName, new[] { action.Workflow });
            newStore.Data.Add(state.Id, state);
        }
        else
        {
            var state = newStore.Data[action.UserName];
            
            var models = new List<InboundWorkflow>(state.Models)
            {
                action.Workflow
            };

            newStore.Data[action.UserName] = (state with
            {
                Models = models.ToArray()
            });
        }

        return newStore;
    }
    [ReducerMethod]
    public static WarehouseInboundStore Remove(WarehouseInboundStore oldStore, WarehouseInboundRemoveAction action)
    {
        var newStore = new WarehouseInboundStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var state = new WarehouseInboundState(action.UserName, Array.Empty<InboundWorkflow>());
            newStore.Data.Add(state.Id, state);
        }
        else
        {
            var state = newStore.Data[action.UserName];
            
            var models = state.Models.Where(x => x.Id != action.WorkflowId);

            newStore.Data[action.UserName] = (state with
            {
                Models = models.ToArray()
            });
        }

        return newStore;
    }
}

using dashboard.Domains.Workflows.Models;

using Fluxor;

namespace dashboard.Domains.Workflows.Store;

public sealed class WorkflowsReducers
{
    [ReducerMethod]
    public static WorkflowsStore Update(WorkflowsStore oldStore, WorkflowsUpdateAction action)
    {
        var newStore = new WorkflowsStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var state = new WorkflowsState(action.UserName, action.Workflows);
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
    public static WorkflowsStore Update(WorkflowsStore oldStore, WorkflowUpdateAction action)
    {
        var newStore = new WorkflowsStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var workflows = new Workflow[] { action.Workflow };

            var state = new WorkflowsState(action.UserName, workflows);
            newStore.Data.Add(state.Id, state);
        }
        else
        {
            var workflows = newStore.Data[action.UserName].Models;
            var workflow = workflows.FirstOrDefault(x => x.Id == action.Workflow.Id);

            if(workflow is not null)
            {
                workflows = workflows
                    .Where(x => x.Id != action.Workflow.Id)
                    .Append(action.Workflow)
                    .OrderByDescending(x => x.Metadata.CreatedOn)
                    .ToArray();
            }
            else
            {
                workflows = workflows
                    .Append(action.Workflow)
                    .OrderByDescending(x => x.Metadata.CreatedOn)
                    .ToArray();
            }

            newStore.Data[action.UserName] = (newStore.Data[action.UserName] with
            {
                Models = workflows
            });
        }

        return newStore;
    }
}

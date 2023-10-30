using Fluxor;

namespace dashboard.Domains._Extra.Documents.Store;

public sealed class DocumentsReducers
{
    [ReducerMethod]
    public static DocumentsStore Update(DocumentsStore oldStore, DocumentsUpdateAction action)
    {
        var newStore = new DocumentsStore(new(oldStore.Data));

        if (!newStore.Data.ContainsKey(action.UserName))
        {
            var state = new DocumentsState(action.UserName, action.Documents);
            newStore.Data.Add(state.Id, state);
        }
        else
        {
            newStore.Data[action.UserName] = (newStore.Data[action.UserName] with
            {
                Models = action.Documents
            });
        }

        return newStore;
    }
}

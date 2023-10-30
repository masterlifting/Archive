using dashboard.Services.Circuits;

using Fluxor;

namespace dashboard.Services.Fluxor.Store.Web;

public sealed class WebEffects
{
    private readonly UserCircuitsService _userCircuits;
    public WebEffects(UserCircuitsService userCircuits) => _userCircuits = userCircuits;

    [EffectMethod]
    public Task CreateWebServices(UserCircuitsServicesCreateAction action, IDispatcher _)
    {
        _userCircuits.CreateServices(action.UserName, action.Circuit, action.Services);
        return Task.CompletedTask;
    }
    [EffectMethod]
    public Task RemoveWebServices(UserCircuitsServicesRemoveAction action, IDispatcher _)
    {
        _userCircuits.RemoveServices(action.Circuit);
        return Task.CompletedTask;
    }
    

    [EffectMethod]
    public Task ShowSnackbar(SnackbarShowAction action, IDispatcher _)
    {
        _userCircuits.ShowSnackbar(action.UserName, action.Message);
        return Task.CompletedTask;
    }
    [EffectMethod]
    public Task ShowDialog(DialogShowAction action, IDispatcher _)
    {
        return _userCircuits.ShowDialog(action.UserName, action.Dialog);
    }
    [EffectMethod]
    public Task CallStateHasChanged(StateHasChangedAction action, IDispatcher _)
    {
       return _userCircuits.CallStateHasChanged(action.UserName);
    }
}

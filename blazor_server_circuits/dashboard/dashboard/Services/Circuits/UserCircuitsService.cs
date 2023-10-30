using System.Collections.Concurrent;

using dashboard.Services.Fluxor.Store.Web.Models;

using Microsoft.AspNetCore.Components.Server.Circuits;

namespace dashboard.Services.Circuits;

public sealed class UserCircuitsService
{
    private readonly ConcurrentDictionary<string, Dictionary<string, UserCircuitServicesModel>> _services = new();

    public void CreateServices(string userId, Circuit? circuit, UserCircuitServicesModel services)
    {
        if (circuit is null)
            return;

        _services.AddOrUpdate(
            userId,
            new Dictionary<string, UserCircuitServicesModel>(3) { { circuit.Id, services } },
            (key, oldValue) =>
            {
                oldValue[circuit.Id] = services;
                return oldValue;
            });
    }
    public void RemoveServices(Circuit circuit)
    {
        foreach (var (_, value) in _services)
        {
            value.Remove(circuit.Id);
        }
    }

    public void ShowSnackbar(string userId, SnackbarMessageModel message)
    {
        if (_services.TryGetValue(userId, out var circuitServices))
        {
            foreach (var services in circuitServices.Values)
            {
                var service = services.Snackbar;

                if (message.WithClear)
                    service.Clear();

                service.Add(message.Text, message.Severity);
            }
        }
    }
    public async Task ShowDialog(string userId, DialogComponentModel dialog)
    {
        if (_services.TryGetValue(userId, out var circuitServices))
        {
            foreach (var services in circuitServices.Values)
            {
                await services.InvokeAsync(() => services.Dialog.Show(dialog.Type, dialog.Title, dialog.Parameters, dialog.options));
            }
        }
    }
    public async Task CallStateHasChanged(string userId)
    {
        if (_services.TryGetValue(userId, out var circuitServices))
        {
            foreach (var (_, services) in circuitServices)
            {
                await services.InvokeAsync(services.StateHasChanged);
            }
        }
    }
}

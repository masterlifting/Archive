using dashboard.Services.Fluxor.Store.Web.Models;

using Microsoft.AspNetCore.Components.Server.Circuits;

namespace dashboard.Services.Fluxor.Store.Web;

public sealed record UserCircuitsServicesCreateAction(string UserName, Circuit? Circuit, UserCircuitServicesModel Services);
public sealed record UserCircuitsServicesRemoveAction(Circuit Circuit);

public sealed record SnackbarShowAction(string UserName, SnackbarMessageModel Message);
public sealed record DialogShowAction(string UserName, DialogComponentModel Dialog);
public sealed record StateHasChangedAction(string UserName);


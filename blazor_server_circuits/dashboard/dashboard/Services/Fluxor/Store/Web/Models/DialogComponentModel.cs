using MudBlazor;

namespace dashboard.Services.Fluxor.Store.Web.Models;

public sealed record DialogComponentModel(Type Type, string Title, DialogParameters Parameters, DialogOptions? options = null);

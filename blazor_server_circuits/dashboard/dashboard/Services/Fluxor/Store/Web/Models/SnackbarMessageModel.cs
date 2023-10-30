using MudBlazor;

namespace dashboard.Services.Fluxor.Store.Web.Models;

public sealed record SnackbarMessageModel(string Text, Severity Severity, bool WithClear = false);

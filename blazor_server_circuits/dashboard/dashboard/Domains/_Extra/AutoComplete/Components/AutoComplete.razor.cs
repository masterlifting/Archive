using dashboard.Domains._Extra.AutoComplete.Models;
using dashboard.Domains._Extra.AutoComplete.Services.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains._Extra.AutoComplete.Components;

public partial class AutoComplete<T> : ComponentBase where T : IAutoComplete
{
    [Parameter] public string Label { get; set; } = default!;
    [Parameter] public EditContext? EditContext { get; set; }
    [Parameter] public Action<T>? HandleResult { get; set; } = default!;
    [Parameter] public Action<T>? OnBlur { get; set; }
    [Parameter] public T? StoredItem { get; set; }
    [Parameter] public Func<T?, string> TToString { get; set; } = default!;

    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;

    [Inject] IAutoCompleteSearch AutoCompleteSearch { get; set; } = null!;

    private MudAutocomplete<T>? _autocomplete = null;
    private MudBlazor.Converter<T, string>? _itemConverter;

    T? _selectedItem = default!;

    ValidationMessageStore? _validationMessageStore = null;

    protected override void OnInitialized()
    {
        _itemConverter = new()
        {
            SetFunc = TToString!
        };

        _selectedItem = StoredItem;

        if (EditContext is not null)
        {
            _validationMessageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += OnValidationRequested;
        }
    }

    private async Task<IEnumerable<T>> Search(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? await AutoCompleteSearch.GetDefaultResults<T>()
            : await AutoCompleteSearch.GetFoundResults<T>(value);
    }

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        var context = (EditContext)sender!;

        _validationMessageStore?.Clear();

        if (_selectedItem is null)
        {
            _validationMessageStore?.Add(() => _selectedItem!, $"{Label} {Localizer["is required"]}");
        }
        else
        {
            if (HandleResult is not null)
                HandleResult?.Invoke(_selectedItem);
        }

        context.NotifyValidationStateChanged();
    }

    private void OnBlurInvoke()
    {
        if (_selectedItem is not null)
        {
            OnBlur?.Invoke(_selectedItem);
        }
    }
}


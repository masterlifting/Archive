using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;
using dashboard.Domains.Warehouse.Inbound.Store;
using dashboard.Shared;

using Fluxor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains.Warehouse.Inbound.Pages;

[Authorize, Route("/warehouse/inbound")]
public partial class Inbound : UserCircuitsComponentBase
{
    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    [Inject] IStateSelection<WarehouseInboundStore, WarehouseInboundState> InboundSelector { get; set; } = null!;
    [Inject] IWarehouseInboundData InboundData { get; set; } = default!;
    [Inject] IWarehouseInboundState InboundState { get; set; } = default!;

    bool _selectorIsReady = false;
    string _userName = string.Empty;
    private InboundWorkflowCategory[] _categories = Array.Empty<InboundWorkflowCategory>();

    InboundWorkflowCreate? _createModel = null;
    ValidationMessageStore? _validationMessageStore = null;

    protected override async Task OnInitializedAsync()
    {
        _userName = await Authorization.GetUserName();
        
        _categories = await InboundData.GetCategories();

        _createModel = await InboundState.Get(_userName);

        if(_categories.Any())
            _createModel.CategoryId = _categories.First().Id;

        var startedWorkflows = await InboundData.GetStarted(_userName);

        Dispatcher.Dispatch(new WarehouseInboundUpdateAction(_userName, startedWorkflows));

        InboundSelector.Select(x => x.Data[_userName]);

        _selectorIsReady = true;
    }

    private void MoveToDetails(DataGridRowClickEventArgs<InboundWorkflow> workflow)
    {
        NavigationManager.NavigateTo($"/warehouse/inbound/{workflow.Item.Id}");
    }
    private async Task OnValidSubmit(EditContext context)
    {
        if(_validationMessageStore is null)
        {
            _validationMessageStore = new ValidationMessageStore(context);
            context.OnValidationRequested += OnWeightValidationRequested;
        }

        if (context.Validate())
        {
            if (string.IsNullOrEmpty(_userName))
                _userName = await Authorization.GetUserName();

            await InboundData.Create(_createModel!);

            _createModel = await InboundData.GetNewCreationModel();

            if (_categories.Any())
                _createModel.CategoryId = _categories.First().Id;

            await InboundState.Clear(_userName);
        }
    }
    private async Task OnSetState()
    {
        if (_createModel is not null)
        {
            if (string.IsNullOrEmpty(_userName))
                _userName = await Authorization.GetUserName();

            await InboundState.Set(_userName, _createModel);
        }
    }

    private void OnWeightValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        var context = (EditContext)sender!;

        _validationMessageStore?.Clear();

        if(string.IsNullOrWhiteSpace(_createModel!.Weight))
        {
            _validationMessageStore?.Add(() => _createModel!.Weight!, $"{Localizer["Weight"]} {Localizer["is required"]}");
            context.NotifyValidationStateChanged();
            return;
        }

        var validationError = InboundData.TryValidateWeight(_createModel!.Weight!, out var weight);
        
        if (validationError is not null)
        {
            _validationMessageStore?.Add(() => _createModel!.Weight!, validationError);
        }
        else
        {
            _createModel.WeightDb = weight;
            
            _validationMessageStore = null;
            context.OnValidationRequested -= OnWeightValidationRequested;
        }

        context.NotifyValidationStateChanged();
    }
}

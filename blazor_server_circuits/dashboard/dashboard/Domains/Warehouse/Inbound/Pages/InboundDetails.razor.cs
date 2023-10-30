using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;
using dashboard.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace dashboard.Domains.Warehouse.Inbound.Pages;

[Authorize, Route("/warehouse/inbound/{Id}")]
public partial class InboundDetails : UserCircuitsComponentBase
{
    [Parameter] public string Id { get; set; } = default!;

    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; } = null!;

    [Inject] IWarehouseInboundData InboundData { get; set; } = default!;

    bool _isLoading = true;
    InboundWorkflowDetails? _details = null;

    protected override async Task OnInitializedAsync()
    {
        _details = await InboundData.GetDetails(Id);
        _isLoading = false;
    }
    private void MoveToDocument(Guid documentId)
    {
        NavigationManager.NavigateTo($"/documents/pdf/{documentId}");
    }
}

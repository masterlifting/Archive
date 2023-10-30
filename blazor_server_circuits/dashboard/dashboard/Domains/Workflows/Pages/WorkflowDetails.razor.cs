using dashboard.Domains.Workflows.Services.Interfaces;
using dashboard.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace dashboard.Domains.Workflows.Pages;

[Authorize, Route("/workflows/{Id}")]
public partial class WorkflowDetails : UserCircuitsComponentBase
{
    [Parameter] public string Id { get; set; } = default!;

    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; } = null!;

    [Inject] IWorkflowsData WorkflowsData { get; set; } = default!;

    bool _isLoading = true;
    Models.WorkflowDetails? _details;

    protected override async Task OnInitializedAsync()
    {
        _details = await WorkflowsData.GetWorkflowDetails(Id);
        _isLoading = false;
    }

    private void MoveToDocument(Guid documentId)
    {
        NavigationManager.NavigateTo($"/documents/pdf/{documentId}");
    }
}

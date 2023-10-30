using dashboard.Domains.Workflows.Models;
using dashboard.Domains.Workflows.Services.Interfaces;
using dashboard.Domains.Workflows.Store;
using dashboard.Shared;

using Fluxor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains.Workflows.Pages;

[Authorize, Route("/workflows")]
public partial class Workflows : UserCircuitsComponentBase
{
    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    [Inject] IStateSelection<WorkflowsStore, WorkflowsState> WorkflowsSelector { get; set; } = null!;
    [Inject] IWorkflowsData WorkflowsData { get; set; } = default!;

    bool _selectorIsReady = false;
    private int _activePanelIndex = 0;
    private string _userName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _userName = await Authorization.GetUserName();

        await LoadUserWorkFlows();
        
        WorkflowsSelector.Select(x => x.Data[_userName]);

        _selectorIsReady = true;
        
    }

    private void MoveToWorkflowDetails(DataGridRowClickEventArgs<Workflow> workflow) => NavigationManager.NavigateTo($"/workflows/{workflow.Item.Id}");

    private async Task LoadUserWorkFlows()
    {
        _activePanelIndex = 0;

        if(string.IsNullOrEmpty(_userName))
            _userName = await Authorization.GetUserName();

        var workflows = await WorkflowsData.GetUserWorkflows(_userName);

        Dispatcher.Dispatch(new WorkflowsUpdateAction(_userName, workflows));
    }
    private async Task LoadUserRolesWorkflows()
    {
        _activePanelIndex = 1;

        if(string.IsNullOrEmpty(_userName))
            _userName = await base.Authorization.GetUserName();

        var workflows = await WorkflowsData.GetUserRolesWorkflows(_userName);

        Dispatcher.Dispatch(new WorkflowsUpdateAction(_userName, workflows));
    }
}

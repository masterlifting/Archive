using System.Reflection.Metadata;

using dashboard.Domains._Extra.Documents.Models;
using dashboard.Domains._Extra.Documents.Services.Interfaces;
using dashboard.Domains._Extra.Documents.Store;
using dashboard.Shared;

using Fluxor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using MudBlazor;

using static dashboard.Domains.Constants.Extra;

namespace dashboard.Domains._Extra.Documents.Pages;

[Authorize]
[Route("/documents")]
[Route("/documents/{Type}")]
[Route("/documents/{Type}/{Id}")]
public partial class DocumentsViewer : UserCircuitsComponentBase
{
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Type { get; set; }

    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] IStateSelection<DocumentsStore, DocumentsState> DocumentsSelector { get; set; } = null!;
    [Inject] IDocumentsViewer ViewerService { get; set; } = null!;
    
    protected override async Task OnInitializedAsync()
    {
        var documents = Array.Empty<DashboardDocument>();
        
        if (Type is null)
        {
            documents = await ViewerService.GetUserDocuments();
        }
        else
        {
            if(!Enum.TryParse<SupportedDocumentTypes>(Type, true, out var type))
                throw new NotSupportedException();

            if(Id is null)
            {
                documents = await ViewerService.GetUserDocuments(type);
            }
            else
            {
                var document = await ViewerService.GetUserDocument(type, Id);
                documents = new[] { document };
            }
        }

        var userName = await base.Authorization.GetUserName();

        base.Dispatcher.Dispatch(new DocumentsUpdateAction(userName, documents));

        DocumentsSelector.Select(x => x.Data[userName]);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (DocumentsSelector.Value.Models.Length == 1)
        {
            await ViewerService.OpenInNewTab(DocumentsSelector.Value.Models[0]);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnRowClick(DataGridRowClickEventArgs<DashboardDocument> row)
    {
        await ViewerService.OpenInNewTab(row.Item);
    }
}

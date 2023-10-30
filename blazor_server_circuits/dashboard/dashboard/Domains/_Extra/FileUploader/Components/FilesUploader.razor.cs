using dashboard.Domains._Extra.FileUploader.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace dashboard.Domains._Extra.FileUploader.Components;

[Authorize]
public partial class FilesUploader : ComponentBase
{
    [Parameter] public string? ButtonName { get; set; }
    [Parameter] public string? ButtonIcon { get; set; }

    [Inject] IStringLocalizer<App> Localizer { get; set; } = null!;
    [Inject] IFilesUploader UploaderService { get; set; } = null!;

    private Task UploadFiles(IReadOnlyList<IBrowserFile> files)
    {
        var uploadTasks = new List<Task>(files.Count);

        foreach (var file in files)
            uploadTasks.Add(UploaderService.UploadFile(file));

        return Task.WhenAll(uploadTasks);
    }
}

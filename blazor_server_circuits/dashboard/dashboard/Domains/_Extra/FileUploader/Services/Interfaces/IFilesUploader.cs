using Microsoft.AspNetCore.Components.Forms;

namespace dashboard.Domains._Extra.FileUploader.Services.Interfaces;

public interface IFilesUploader
{
    Task UploadFile(IBrowserFile file);
}

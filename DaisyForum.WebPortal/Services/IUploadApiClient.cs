using DaisyForum.ViewModels;

namespace DaisyForum.WebPortal.Services
{
    public interface IUploadApiClient
    {
        Task Upload(UploadViewModel request);
    }
}
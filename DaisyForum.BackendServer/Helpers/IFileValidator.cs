using Microsoft.AspNetCore.Http;

namespace DaisyForum.BackendServer.Helpers
{
    public interface IFileValidator
    {
        bool IsValid(IFormFile file);
    }
}

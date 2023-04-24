using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface ICategoryApiClient
{
    Task<List<CategoryViewModel>> GetCategories();
}
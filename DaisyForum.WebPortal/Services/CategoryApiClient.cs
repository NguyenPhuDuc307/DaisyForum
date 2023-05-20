using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public class CategoryApiClient : BaseApiClient, ICategoryApiClient
{
    public CategoryApiClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
    {
    }

    public async Task<List<CategoryViewModel>> GetCategories()
    {
        return await GetListAsync<CategoryViewModel>("/api/categories");
    }

    public async Task<CategoryViewModel> GetCategoryById(int id)
    {
        return await GetAsync<CategoryViewModel>($"/api/categories/{id}");
    }
}
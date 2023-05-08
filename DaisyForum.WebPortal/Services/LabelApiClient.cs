using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public class LabelApiClient : BaseApiClient, ILabelApiClient
{
    public LabelApiClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
    {
    }

    public async Task<LabelViewModel> GetLabelById(string labelId)
    {
        return await GetAsync<LabelViewModel>($"/api/labels/{labelId}");
    }

    public async Task<List<LabelViewModel>> GetPopularLabels(int take)
    {
        return await GetListAsync<LabelViewModel>($"/api/labels/popular/{take}");
    }
}
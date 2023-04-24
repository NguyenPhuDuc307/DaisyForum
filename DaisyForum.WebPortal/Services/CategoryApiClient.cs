using DaisyForum.ViewModels.Contents;
using Newtonsoft.Json;

namespace DaisyForum.WebPortal.Services;

public class CategoryApiClient : ICategoryApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CategoryApiClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<CategoryViewModel>> GetCategories()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/api/categories");
        var categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(await response.Content.ReadAsStringAsync());
        return categories;
    }
}
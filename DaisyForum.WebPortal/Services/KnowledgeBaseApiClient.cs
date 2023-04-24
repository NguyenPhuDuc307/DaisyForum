using DaisyForum.ViewModels.Contents;
using Newtonsoft.Json;

namespace DaisyForum.WebPortal.Services;

public class KnowledgeBaseApiClient : IKnowledgeBaseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public KnowledgeBaseApiClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<KnowledgeBaseQuickViewModel>> GetLatestKnowledgeBases(int take)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"/api/knowledgeBases/latest/{take}");
        var latestKnowledgeBases = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickViewModel>>(await response.Content.ReadAsStringAsync());
        return latestKnowledgeBases;
    }

    public async Task<List<KnowledgeBaseQuickViewModel>> GetPopularKnowledgeBases(int take)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"/api/knowledgeBases/popular/{take}");
        var latestKnowledgeBases = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickViewModel>>(await response.Content.ReadAsStringAsync());
        return latestKnowledgeBases;
    }

    public async Task<List<KnowledgeBaseQuickViewModel>> GetPopularLabels(int take)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        //var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"/api/knowledgeBases/popular/{take}");
        var popularKnowledgeBases = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickViewModel>>(await response.Content.ReadAsStringAsync());
        return popularKnowledgeBases;
    }
}
using System.Net.Http.Headers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Authentication;

namespace DaisyForum.WebPortal.Services;

public class UserApiClient : BaseApiClient, IUserApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserApiClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, configuration, httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserViewModel> GetById(string id)
    {
        return await GetAsync<UserViewModel>($"/api/users/{id}", true);
    }

    public async Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByUserId(string userId, int pageIndex, int pageSize)
    {
        var apiUrl = $"/api/users/{userId}/knowledgeBases?pageIndex={pageIndex}&pageSize={pageSize}";
        return await GetAsync<Pagination<KnowledgeBaseQuickViewModel>>(apiUrl, true);
    }

    public async Task<List<KnowledgeBaseQuickViewModel>> GetKnowledgeSuggested(string userId, int size = 5)
    {
        var apiUrl = $"/api/users/suggested?userId={userId}&size={size}";
        return await GetAsync<List<KnowledgeBaseQuickViewModel>>(apiUrl, true);
    }

    public async Task<Pagination<LabelViewModel>> GetLabels(string? keyword, int pageIndex, int pageSize)
    {
        var apiUrl = $"/api/labels/filter?keyword={keyword}&page={pageIndex}&pageSize={pageSize}";
        return await GetAsync<Pagination<LabelViewModel>>(apiUrl);
    }

    public async Task<bool> PutLabels(string userId, string[] labels)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");

        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        using var requestContent = new MultipartFormDataContent();

        foreach (var label in labels)
        {
            requestContent.Add(new StringContent(label), "labels");
        }

        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsync($"/api/users/{userId}/putlabels", requestContent);
        return response.IsSuccessStatusCode;
    }
}
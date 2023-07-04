using System.Net.Http.Headers;
using DaisyForum.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace DaisyForum.WebPortal.Services;

public class UploadApiClient : BaseApiClient, IUploadApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadApiClient(IHttpClientFactory httpClientFactory,
      IConfiguration configuration,
      IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Upload(UploadViewModel request)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");

        client.BaseAddress = new Uri(_configuration["BackendApiUrl"]);
        using var requestContent = new MultipartFormDataContent();

        if (request.File != null)
        {
            byte[] data;
            using (var br = new BinaryReader(request.File.OpenReadStream()))
            {
                data = br.ReadBytes((int)request.File.OpenReadStream().Length);
            }
            ByteArrayContent bytes = new ByteArrayContent(data);
            requestContent.Add(bytes, "file", request.File.FileName);

        }
        requestContent.Add(new StringContent(request.RoomId.ToString()), "RoomId");

        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsync($"/api/knowledgeBases/", requestContent);
    }
}
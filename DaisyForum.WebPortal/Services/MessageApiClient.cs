using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public class MessageApiClient : BaseApiClient, IMessageApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MessageApiClient(IHttpClientFactory httpClientFactory,
      IConfiguration configuration,
      IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<MessageViewModel> Create(MessageViewModel request)
    {
        return await PostAsync<MessageViewModel, MessageViewModel>($"/api/messages/", request);
    }

    public async Task Delete(int id)
    {
        await DeleteAsync<bool>($"/api/messages/{id}");
    }

    public async Task<MessageViewModel> Get(int id)
    {
        return await GetAsync<MessageViewModel>($"/api/messages/{id}");
    }

    public async Task<List<MessageViewModel>> GetMessages(string roomName)
    {
        return await GetListAsync<MessageViewModel>($"/api/messages/roomName/{roomName}");
    }
}
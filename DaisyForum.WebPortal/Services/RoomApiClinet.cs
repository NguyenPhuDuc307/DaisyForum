using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public class RoomApiClient : BaseApiClient, IRoomApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoomApiClient(IHttpClientFactory httpClientFactory,
      IConfiguration configuration,
      IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<RoomViewModel> Create(RoomViewModel request)
    {
        return await PostAsync<RoomViewModel, RoomViewModel>($"/api/rooms/", request);
    }

    public async Task Delete(int id)
    {
        await DeleteAsync<bool>($"/api/rooms/{id}");
    }

    public async Task Edit(int id, RoomViewModel request)
    {
        await PutAsync<RoomViewModel, RoomViewModel>($"/api/rooms/{id}", request);
    }

    public async Task<List<RoomViewModel>> Get()
    {
        return await GetListAsync<RoomViewModel>($"/api/rooms/");
    }

    public async Task<RoomViewModel> Get(int id)
    {
        return await GetAsync<RoomViewModel>($"/api/rooms/{id}");
    }
}
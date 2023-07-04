using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface IRoomApiClient
{
    Task<List<RoomViewModel>> Get();

    Task<RoomViewModel> Get(int id);

    Task<RoomViewModel> Create(RoomViewModel request);

    Task Edit(int id, RoomViewModel request);

    Task Delete(int id);
}
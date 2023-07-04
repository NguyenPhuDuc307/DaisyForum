using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface IMessageApiClient
{
    Task<MessageViewModel> Get(int id);

    Task<List<MessageViewModel>> GetMessages(string roomName);

    Task<MessageViewModel> Create(MessageViewModel request);

    Task Delete(int id);
}
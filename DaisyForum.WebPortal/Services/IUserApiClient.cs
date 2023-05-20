using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;

namespace DaisyForum.WebPortal.Services;
public interface IUserApiClient
{
    Task<UserViewModel> GetById(string id);
    Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByUserId(string userId, int pageIndex, int pageSize);
}

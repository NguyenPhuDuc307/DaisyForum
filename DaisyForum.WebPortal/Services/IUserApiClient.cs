using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;

namespace DaisyForum.WebPortal.Services;
public interface IUserApiClient
{
    Task<UserViewModel> GetById(string id);
    Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByUserId(string userId, int pageIndex, int pageSize);
    Task<Pagination<LabelViewModel>> GetLabels(string? keyword, int pageIndex, int pageSize);
    Task<bool> PutLabels(string userId, string[] labels);
    Task<List<KnowledgeBaseQuickViewModel>> GetKnowledgeSuggested(string userId, int size = 5);
}

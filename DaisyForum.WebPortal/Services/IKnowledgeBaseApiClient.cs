using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface IKnowledgeBaseApiClient
{
    Task<List<KnowledgeBaseQuickViewModel>> GetPopularKnowledgeBases(int take);
    Task<List<KnowledgeBaseQuickViewModel>> GetLatestKnowledgeBases(int take);
    Task<List<LabelViewModel>> GetPopularLabels(int take);
}
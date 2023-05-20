using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Services;

public interface IKnowledgeBaseApiClient
{
    Task<List<KnowledgeBaseQuickViewModel>> GetPopularKnowledgeBases(int take);

    Task<List<KnowledgeBaseQuickViewModel>> GetLatestKnowledgeBases(int take);

    Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByCategoryId(int categoryId, int pageIndex, int pageSize);

    Task<Pagination<KnowledgeBaseQuickViewModel>> SearchKnowledgeBase(string keyword, int pageIndex, int pageSize);

    Task<Pagination<KnowledgeBaseQuickViewModel>> GetKnowledgeBasesByTagId(string tagId, int pageIndex, int pageSize);

    Task<KnowledgeBaseViewModel> GetKnowledgeBaseDetail(int id);

    Task<List<LabelViewModel>> GetLabelsByKnowledgeBaseId(int id);

    Task<List<CommentViewModel>> GetRecentComments(int take);

    Task<Pagination<CommentViewModel>> GetCommentsTree(int knowledgeBaseId, int pageIndex, int pageSize);

    Task<Pagination<CommentViewModel>> GetRepliedComments(int knowledgeBaseId, int rootCommentId, int pageIndex, int pageSize);

    Task<CommentViewModel> PostComment(CommentCreateRequest request);

    Task<bool> PostKnowledgeBase(KnowledgeBaseCreateRequest request);

    Task<bool> PutKnowledgeBase(int id, KnowledgeBaseCreateRequest request);

    Task<bool> UpdateViewCount(int id);

    Task<int> PostVote(VoteCreateRequest request);

    Task<ReportViewModel> PostReport(ReportCreateRequest request);
}
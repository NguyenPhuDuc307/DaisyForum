using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Models;

public class SideBarViewModel
{
    public List<KnowledgeBaseQuickViewModel>? PopularKnowledgeBases { get; set; }
    public List<CategoryViewModel>? Categories { get; set; }
    public List<CommentViewModel>? RecentComments { get; set; }
}
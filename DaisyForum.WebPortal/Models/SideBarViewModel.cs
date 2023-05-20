using DaisyForum.ViewModels.Contents;
using static DaisyForum.WebPortal.Controllers.Components.SideBarViewComponent;

namespace DaisyForum.WebPortal.Models;

public class SideBarViewModel
{
    public List<KnowledgeBaseQuickViewModel>? PopularKnowledgeBases { get; set; }
    public List<TreeNode>? TreeNodes { get; set; }
    public List<CommentViewModel>? RecentComments { get; set; }
}
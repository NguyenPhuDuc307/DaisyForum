using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;

namespace DaisyForum.WebPortal.Models;

public class KnowledgeBaseDetailViewModel
{
    public CategoryViewModel? Category { set; get; }
    public KnowledgeBaseViewModel? Detail { get; set; }
    public List<LabelViewModel>? Labels { get; set; }
    public UserViewModel? CurrentUser { get; set; }
}
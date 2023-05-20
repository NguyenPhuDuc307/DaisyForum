using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Models;

public class ListByTagIdViewModel
{
    public Pagination<KnowledgeBaseQuickViewModel>? Data { set; get; }
    public LabelViewModel? LabelVm { set; get; }
}
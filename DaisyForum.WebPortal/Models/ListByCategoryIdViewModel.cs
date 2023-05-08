using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Models;

public class ListByCategoryIdViewModel
{
    public Pagination<KnowledgeBaseQuickViewModel>? Data { set; get; }
    public CategoryViewModel? Category { set; get; }
}
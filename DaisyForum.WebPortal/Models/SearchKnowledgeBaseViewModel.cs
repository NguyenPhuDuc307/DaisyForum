using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Models
{
    public class SearchKnowledgeBaseViewModel
    {
        public Pagination<KnowledgeBaseQuickViewModel>? Data { set; get; }

        public string? Keyword { set; get; }
    }
}
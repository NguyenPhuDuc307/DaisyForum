using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Models
{
    public class HomeViewModel
    {
        public List<KnowledgeBaseQuickViewModel>? LatestKnowledgeBases { get; set; }
        public List<KnowledgeBaseQuickViewModel>? PopularKnowledgeBases { get; set; }
        public List<LabelViewModel>? PopularLabels { get; set; }
    }
}
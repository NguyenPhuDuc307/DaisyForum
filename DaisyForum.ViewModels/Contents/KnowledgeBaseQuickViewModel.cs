namespace DaisyForum.ViewModels.Contents
{
    public class KnowledgeBaseQuickViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Title { get; set; }
        public string? SeoAlias { get; set; }
        public string? Description { get; set; }
    }
}
namespace DaisyForum.ViewModels.Contents
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SeoAlias { get; set; }
        public string? SeoDescription { get; set; }
        public int SortOrder { get; set; }
        public int? ParentId { get; set; }
        public int? NumberOfTickets { get; set; }
        public List<CategoryViewModel>? Children { get; set; }
    }
}
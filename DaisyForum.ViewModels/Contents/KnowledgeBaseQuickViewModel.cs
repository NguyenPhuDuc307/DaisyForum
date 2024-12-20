namespace DaisyForum.ViewModels.Contents;

public class KnowledgeBaseQuickViewModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryAlias { get; set; }
    public string? CategoryName { get; set; }
    public string? Title { get; set; }
    public string? SeoAlias { get; set; }
    public string? Description { get; set; }
    public int? ViewCount { get; set; } = 0;
    public DateTime CreateDate { get; set; }
    public int? NumberOfVotes { get; set; } = 0;
    public int? NumberOfComments { get; set; } = 0;
    public int? NumberOfReports { get; set; } = 0;
    public string[]? Labels { get; set; }
    public bool? IsProcessed { get; set; }
}
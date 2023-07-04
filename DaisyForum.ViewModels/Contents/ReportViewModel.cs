namespace DaisyForum.ViewModels.Contents
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string? ReportUserName { get; set; }
        public int? KnowledgeBaseId { get; set; }
        public string? KnowledgeTitle { get; set; }
        public string? Content { get; set; }
        public string? ReportUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsProcessed { get; set; }
        public string? Type { get; set; }
    }
}
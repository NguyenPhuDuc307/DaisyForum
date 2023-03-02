namespace DaisyForum.ViewModels.Contents
{
    public class VoteViewModel
    {
        public int KnowledgeBaseId { get; set; }
        public string? UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
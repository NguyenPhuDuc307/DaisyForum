namespace DaisyForum.ViewModels.Contents;

public class CommentViewModel
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public int KnowledgeBaseId { get; set; }
    public string? KnowledgeBaseTitle { get; set; }
    public string? KnowledgeBaseSeoAlias { get; set; }
    public string? OwnerUserId { get; set; }
    public string? OwnerName { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int? ReplyId { get; set; }
    public float NavigationScore { get; set; }
    public string? Note { get; set; }
    public Pagination<CommentViewModel> Children { get; set; } = new Pagination<CommentViewModel>();
}
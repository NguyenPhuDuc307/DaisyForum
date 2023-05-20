namespace DaisyForum.BackendServer.Models;

public class RepliedCommentViewModel
{
    public string? RepliedName { get; set; }

    public string? CommentContent { get; set; }

    public string? KnowledgeBaseTitle { get; set; }

    public int KnowledgeBaseId { get; set; }

    public string? KnowledgeBaseSeoAlias { get; set; }
}
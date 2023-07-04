using System.Collections;

namespace DaisyForum.ViewModels.Contents;

public class KnowledgeBasesFromCSV
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string[]? Tags { get; set; }
    public DateTime CreationDate { get; set; }
    public string? Y { get; set; }
}

public class KnowledgeBasesVector
{
    public int Id { get; set; }
    public BitArray? Vector { get; set; }
}

public class UserBases
{
    public string? UserId { get; set; }
    public string[]? Tags { get; set; }
    public int? PostId { get; set; }
    public double? Cosine { get; set; }
}
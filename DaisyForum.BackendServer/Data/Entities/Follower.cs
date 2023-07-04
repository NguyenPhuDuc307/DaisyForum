using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DaisyForum.BackendServer.Data.Entities;

[Table("Followers")]
public class Follower
{
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? OwnerUserId { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? FollowerId { get; set; }
    public DateTime CreateDate { get; set; }
    public bool Notification { get; set; }
}
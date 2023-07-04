using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaisyForum.BackendServer.Data.Entities;

[Table("Messages")]
public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(1000)]
    [Required]
    public string? Content { get; set; }
    public DateTime Timestamp { get; set; }
    public User? FromUser { get; set; }
    public int ToRoomId { get; set; }
    public Room? ToRoom { get; set; }
    public bool Status { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaisyForum.BackendServer.Data.Entities
{
    [Table("Rooms")]
    public class Room
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public User? Admin { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReminderManager.Domain.Entities
{
    [Table("users")]
    public class User
    {

        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();


        [Column("username")]

        public required string Username { get; set; }


        [Column("password")]
        public required string Password { get; set; }

    }
}

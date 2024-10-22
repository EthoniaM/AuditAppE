using System.ComponentModel.DataAnnotations;

namespace KanBan.Model
{
    public class User
    {
        [Key]
            public int UsersID { get; set; } 

            [Required]
            [StringLength(50)]
            public string FirstName { get; set; } 

            [Required]
            [StringLength(50)]
            public string LastName { get; set; } 

            [Required]
            [EmailAddress]
            [StringLength(100)]
            public string Email { get; set; } 

            [Required]
            [StringLength(100)]
            public string Password { get; set; } 

            [Required]
            public string Role { get; set; } 

            public virtual ICollection<KanbanDetail> KanbanDetails { get; set; } = new List<KanbanDetail>(); // Navigation property
        }
    }


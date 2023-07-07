using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class User : UserIdentity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(30, MinimumLength = 4)]
        public string Email { get; set; } = string.Empty;
    }
}

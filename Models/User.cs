using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class User : UserIdentity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Not email format.")]
        [StringLength(30, MinimumLength = 4)]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
	}
}

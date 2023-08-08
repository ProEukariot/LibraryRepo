using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class UserIdentity
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(12 ,MinimumLength = 4)]
        [RegularExpression(@"\w+")]
		[MaxLength(20)]
		public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Password")]
        [StringLength(12, MinimumLength = 4)]
        [RegularExpression(@"^\w*(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])\w*$")]
		[MaxLength(20)]
		public string Password { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LibraryApp.Models
{
    public class UserIdentityReg : User
    {
        [Required]
        [Display(Name = "Repeat password")]
        [Compare("Password")]
        [StringLength(12, MinimumLength = 4)]
        public string RepeatedPassword { get; set; } = string.Empty;
    }
}

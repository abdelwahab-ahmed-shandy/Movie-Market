using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Identity
{
    public class UserUpdateVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Username required")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First Name required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Address")]

        public string Address { get; set; }

        [Display(Name = "Birth Date")]

        [DataType(DataType.Date)]

        public DateTime? BirthDay { get; set; }

        [Display(Name = "Profile Picture")]

        public IFormFile ProfileImage { get; set; }

        public string ExistingProfileImage { get; set; }
    }
}

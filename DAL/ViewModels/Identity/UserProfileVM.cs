namespace DAL.ViewModels.Identity
{
    public class UserProfileVM
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Address { get; set; }
        public DateTime? BirthDay { get; set; }
        public string ProfileImage { get; set; }
    }
}

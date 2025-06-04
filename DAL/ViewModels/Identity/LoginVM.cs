namespace DAL.ViewModels.Identity
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username or email required")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

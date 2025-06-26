using DAL.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM registerVM);
        Task<SignInResult> LoginAsync(LoginVM loginVM);
        Task LogoutAsync();
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task ForgotPasswordAsync(ForgetPasswordVM forgotPasswordVM);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordVM resetPasswordVM);
    }
}

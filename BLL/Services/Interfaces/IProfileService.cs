using DAL.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileVM> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(ProfileVM model);
        Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordVM model);
        Task<IdentityResult> DeleteAccountAsync(Guid userId);
    }
}

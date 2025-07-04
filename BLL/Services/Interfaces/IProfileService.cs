
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

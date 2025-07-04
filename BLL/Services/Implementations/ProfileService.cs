
namespace BLL.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileService( IApplicationUserRepository userRepository,IFileService fileService ,
                                        UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _fileService = fileService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<ProfileVM> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            return new ProfileVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                BirthDay = user.BirthDay,
                ProfileImagePath = user.ProfileImage
            };
        }

        public async Task UpdateProfileAsync(ProfileVM model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);

            if (user == null)
                throw new Exception("User not found");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.BirthDay = model.BirthDay;

            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    _fileService.DeleteFile(user.ProfileImage);
                }

                // Save new image
                user.ProfileImage = await _fileService.SaveFileAsync(
                    model.ProfileImageFile,
                    "uploads/profile-images");
            }

            await _userRepository.Update(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordVM model)
        {
            if (model == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid password data" });
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }

        public async Task<IdentityResult> DeleteAccountAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            if (await _userManager.IsInRoleAsync(user, "Admin") ||await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Admins cannot delete their accounts."
                });
            }

            await _userManager.UpdateSecurityStampAsync(user);
            return await _userManager.DeleteAsync(user);
        }


    }
}

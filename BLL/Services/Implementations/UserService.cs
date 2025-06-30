using DAL.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IApplicationUserRepository _userRepository;

        public UserService(IApplicationUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserIndexVM>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserIndexVM
            {
                Id = u.Id,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}",
                UserType = u.UserType.ToString(),
                AccountStatus = u.AccountStateType.ToString(),
                RegistrationDate = u.RegistrationDate,
                LastLogin = u.LastLogin
            }).ToList();
        }

        public async Task<List<UserIndexVM>> GetAdminsAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Where(u => u.UserType == UserType.Admin)
                       .Select(u => new UserIndexVM
                       {
                           Id = u.Id,
                           Email = u.Email,
                           FullName = $"{u.FirstName} {u.LastName}",
                           UserType = u.UserType.ToString(),
                           AccountStatus = u.AccountStateType.ToString(),
                           RegistrationDate = u.RegistrationDate,
                           LastLogin = u.LastLogin
                       }).ToList();
        }

        public async Task<List<UserIndexVM>> GetSuperAdminsAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Where(u => u.UserType == UserType.SuperAdmin)
                       .Select(u => new UserIndexVM
                       {
                           Id = u.Id,
                           Email = u.Email,
                           FullName = $"{u.FirstName} {u.LastName}",
                           UserType = u.UserType.ToString(),
                           AccountStatus = u.AccountStateType.ToString(),
                           RegistrationDate = u.RegistrationDate,
                           LastLogin = u.LastLogin
                       }).ToList();
        }

        public async Task<List<UserIndexVM>> GetCustomersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Where(u => u.UserType == UserType.Customer)
                       .Select(u => new UserIndexVM
                       {
                           Id = u.Id,
                           Email = u.Email,
                           FullName = $"{u.FirstName} {u.LastName}",
                           UserType = u.UserType.ToString(),
                           AccountStatus = u.AccountStateType.ToString(),
                           RegistrationDate = u.RegistrationDate,
                           LastLogin = u.LastLogin
                       }).ToList();
        }

        public async Task<UserDetailsVM> GetUserDetailsAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return new UserDetailsVM
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType.ToString(),
                AccountStatus = user.AccountStateType.ToString(),
                Address = user.Address,
                BirthDay = user.BirthDay,
                ProfileImage = user.ProfileImage,
                BlockReason = user.BlockReason,
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin,
                PasswordChangedDate = user.PasswordChangedDate
            };
        }

        public async Task BlockUserAsync(Guid userId, string blockReason, string blockedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            if (user.Email == blockedBy)
            {
                throw new InvalidOperationException("You cannot block yourself!");
            }

            var currentUser = await _userRepository.GetOneAsync(u => u.Email == blockedBy);
            if (currentUser != null &&
                currentUser.UserType == UserType.Admin &&
                (user.UserType == UserType.Admin || user.UserType == UserType.SuperAdmin))
            {
                throw new UnauthorizedAccessException("Admins can only block customers!");
            }

            user.IsBlocked = true;
            user.BlockReason = blockReason;
            user.AccountStateType = AccountStateType.Blocked;

            await _userRepository.Update(user);
        }

        public async Task UnblockUserAsync(Guid userId, string unblockedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsBlocked = false;
                user.BlockReason = null;
                user.AccountStateType = AccountStateType.Active;

                await _userRepository.Update(user);
            }
        }


        public async Task ChangeUserRoleAsync(ChangeUserRoleVM model, string changedBy)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null)
                throw new ArgumentException("User not found");

            var currentUser = await _userRepository.GetOneAsync(u => u.Email == changedBy);
            if (currentUser == null || currentUser.UserType != UserType.SuperAdmin)
                throw new UnauthorizedAccessException("Only SuperAdmins can change user roles");



            if (user.UserType == UserType.SuperAdmin)
                throw new InvalidOperationException("Cannot modify SuperAdmin role");

            user.UserType = model.NewRole;
            await _userRepository.Update(user);
        }

        public async Task<ChangeUserRoleVM> GetUserRoleInfoAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new ChangeUserRoleVM
            {
                UserId = user.Id,
                CurrentEmail = user.Email,
                CurrentRole = user.UserType.ToString(),
                NewRole = user.UserType
            };
        }

    }
}

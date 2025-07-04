using DAL.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IApplicationUserRepository userRepository,UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole<Guid>> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public async Task<PaginatedList<UserIndexVM>> GetAllUsersAsync(int pageIndex = 1, string search = null)
        {
            var users = _userRepository.GetAll(); // IQueryable

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                users = users.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var userVMs = users.Select(user => new UserIndexVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserType = _userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? "None",
                AccountStatus = user.AccountStateType.ToString(),
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin
            });

            return await PaginatedList<UserIndexVM>.CreateAsync(userVMs, pageIndex, 10);
        }

        public async Task<PaginatedList<UserIndexVM>> GetAdminsAsync(int pageIndex = 1, int pageSize = 10, string search = null)
        {
            //  Get IDs of users in Admin role
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminUserIds = adminUsers.Select(u => u.Id).ToList();

            //  Start from IQueryable
            var usersQuery = _userRepository.GetAll().Where(u => adminUserIds.Contains(u.Id));

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            //  Projection to VM
            var userVMs = usersQuery.Select(user => new UserIndexVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserType = "Admin",
                AccountStatus = user.AccountStateType.ToString(),
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin
            });

            //  Paginate
            return await PaginatedList<UserIndexVM>.CreateAsync(userVMs, pageIndex, pageSize);
        }


        public async Task<PaginatedList<UserIndexVM>> GetSuperAdminsAsync(int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");

            var filtered = superAdmins.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                filtered = filtered.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var userVMs = filtered.Select(user => new UserIndexVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserType = "SuperAdmin",
                AccountStatus = user.AccountStateType.ToString(),
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin
            }).ToList();

            return PaginatedList<UserIndexVM>.Create(userVMs, pageIndex, pageSize);
        }

        public async Task<PaginatedList<UserIndexVM>> GetCustomersAsync(int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");

            var filtered = customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                filtered = filtered.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var userVMs = filtered.Select(user => new UserIndexVM
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserType = "Customer",
                AccountStatus = user.AccountStateType.ToString(),
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin
            }).ToList();

            return PaginatedList<UserIndexVM>.Create(userVMs, pageIndex, pageSize);
        }

        public async Task<UserDetailsVM> GetUserDetailsAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "None";

            return new UserDetailsVM
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = role,
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



        public async Task BlockUserAsync(Guid userId, string blockReason)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return;

            var currentEmail = GetCurrentUserEmail();
            if (user.Email == currentEmail)
                throw new InvalidOperationException("You cannot block yourself!");

            var currentUser = await _userManager.FindByEmailAsync(currentEmail);
            if (currentUser == null)
                throw new UnauthorizedAccessException("Current user not found.");

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var targetRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains("SuperAdmin") &&
                (targetRoles.Contains("Admin") || targetRoles.Contains("SuperAdmin")))
            {
                throw new UnauthorizedAccessException("Admins can only block Customers!");
            }

            user.IsBlocked = true;
            user.BlockReason = blockReason;
            user.AccountStateType = AccountStateType.Blocked;

            await _userManager.UpdateAsync(user);
        }

        public async Task UnblockUserAsync(Guid userId)
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
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            var currentUser = await _userManager.FindByEmailAsync(changedBy);
            if (currentUser == null || !(await _userManager.IsInRoleAsync(currentUser, "SuperAdmin")))
                throw new UnauthorizedAccessException("Only SuperAdmins can change user roles");

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                throw new InvalidOperationException("Cannot modify SuperAdmin role");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!await _roleManager.RoleExistsAsync(model.NewRole.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(model.NewRole.ToString()));
            }

            await _userManager.AddToRoleAsync(user, model.NewRole.ToString());
        }

        public async Task<ChangeUserRoleVM> GetUserRoleInfoAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault() ?? "None";

            return new ChangeUserRoleVM
            {
                UserId = user.Id,
                CurrentEmail = user.Email,
                CurrentRole = currentRole,
                NewRole = currentRole 
            };
        }


    }
}

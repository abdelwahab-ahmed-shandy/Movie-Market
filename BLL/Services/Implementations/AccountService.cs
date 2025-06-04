using DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public AccountService(SignInManager<ApplicationUser> signInManager,
                                    IAuditService auditService,
                                        IEmailService emailService,
                                            ILogger<AccountService> logger,
                                                    IUserStore<ApplicationUser> userStore,
                                                            RoleManager<IdentityRole<Guid>> roleManager,
                                                                UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _auditService = auditService;
            _emailService = emailService;
            _logger = logger;
            _userStore = userStore;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterVM model)
        {
            //await EnsureRolesExist();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDay = model.BirthDay,
                Address = model.Address,
                UserType = UserType.User,
                AccountStateType = AccountStateType.PendingActivation,
                IsActive = false,
                RegistrationDate = DateTime.UtcNow
            };

            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                await _userManager.AddToRoleAsync(user, "User");

                await RecordAuditLogAsync("User Registration",
                    $"New user registered with email: {user.Email}",
                    user.Id);

                await SendEmailConfirmationAsync(user);
            }

            return result;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task RecordAuditLogAsync(string action, string description, Guid? userId = null)
        {
            try
            {
                var currentUser = _auditService.GetCurrentUserName();

                _auditService.RecordAuditAsync(
                    userId ?? Guid.Empty,
                    action,
                    description,
                    currentUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording audit log");
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        #region Private Methods
        private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://localhost:7202/Identity/Account/ConfirmEmail?userId={user.Id}&token={token}";

                var emailSubject = "Confirm your email";
                var emailBody = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email");
                throw;
            }
        }

        //private async Task EnsureRolesExist()
        //{
        //    if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
        //        await _roleManager.CreateAsync(new IdentityRole<Guid>("SuperAdmin"));

        //    if (!await _roleManager.RoleExistsAsync("Admin"))
        //        await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

        //    if (!await _roleManager.RoleExistsAsync("User"))
        //        await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
        //}


        #endregion

    }
}

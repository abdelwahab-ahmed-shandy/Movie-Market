using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UsersController : BaseController
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IApplicationUserRepository applicationUserRepository,
                                     ILogger<UsersController> logger, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _applicationUserRepository = applicationUserRepository;
            _userManager = userManager;
            _logger = logger;
        }





    }
}

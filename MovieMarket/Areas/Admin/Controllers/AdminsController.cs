using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMarket.Repositories.IRepositories;

namespace MovieMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuberAdmin")]
    public class AdminsController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminsController(IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager)
        {
            this._applicationUserRepository = applicationUserRepository;
            this._userManager = userManager;
        }

        #region View Admin
        public async Task<IActionResult> AllAdmins(string? query, int page)
        {
            IEnumerable<ApplicationUser> allUsers = _applicationUserRepository.Get().AsNoTracking().ToList();

            var allAdmins = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var role = await _userManager.GetRolesAsync(user);

                if (!role.Contains("SuberAdmin") && role.Contains("Admin"))
                {
                    allAdmins.Add(user);
                }
            }

            if (query != null)
            {
                allAdmins = allAdmins.Where(e => (e.UserName ?? "").Contains(query)
                                                          || (e.Email ?? "").Contains(query)
                                                          || (e.FirstName ?? "").Contains(query)
                                                          || (e.LastName ?? "").Contains(query)
                                                          || (e.Address ?? "").Contains(query)
                                                          || (e.Id ?? "").Contains(query))
                                                          .ToList();
            }

            // Calculate the number of pages required, so that there are 5 Admins per page
            int pageSize = 5;
            int totalCustomers = allAdmins.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            // Check if the requested page does not exist
            if (page > totalPages && totalPages > 0)
                return NotFound();

            // Split the Admins and display only 5 per page
            allAdmins = allAdmins.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the number of pages to the View
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;



            return View(allAdmins.ToList());
        }

        #endregion

        #region Create New Admin

        // Display the new Admin creation form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // Process a new Admin creation request when the form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken] // Protection against CSRF attacks
        public async Task<IActionResult> Create(ApplicationUser applicationUser)
        {
            // Validate the entered data before saving
            if (ModelState.IsValid)
            {
                // Add the new Admin to the database
                _applicationUserRepository.Create(applicationUser);

                // Assign the "Admin" role to the new user
                await _userManager.AddToRoleAsync(applicationUser, "Admin");

                // Save changes to the database
                _applicationUserRepository.SaveDB();

                // Store a success message in TempData to display after redirection
                TempData["notifiction"] = "The Customer was created successfully!";
                TempData["MessageType"] = "success";

                // Redirect to the All Admins view page
                return RedirectToAction(nameof(AllAdmins));
            }

            // If there is an input error, re-render the form with the same data
            return View(applicationUser);
        }

        #endregion

    }
}

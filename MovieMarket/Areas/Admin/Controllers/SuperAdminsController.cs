using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMarket.Repositories.IRepositories;

namespace MovieMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuberAdmin")]
    public class SuperAdminsController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SuperAdminsController(IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            this._applicationUserRepository = applicationUserRepository;
        }

        #region View Super Admin
        public async Task<IActionResult> AllSuperAdmins(string? query, int page)
        {

            IEnumerable<ApplicationUser> allUsers = _applicationUserRepository.Get().AsNoTracking().ToList();

            var allSuberAdmin = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var role = await _userManager.GetRolesAsync(user);

                if (role.Contains("SuberAdmin"))
                {
                    allSuberAdmin.Add(user);
                }
            }

            if (query != null)
            {
                allSuberAdmin = allSuberAdmin.Where(e => (e.UserName ?? "").Contains(query)
                                                          || (e.Email ?? "").Contains(query)
                                                          || (e.FirstName ?? "").Contains(query)
                                                          || (e.LastName ?? "").Contains(query)
                                                          || (e.Address ?? "").Contains(query)
                                                          || (e.Id ?? "").Contains(query))
                                                          .ToList();
            }


            // Calculate the number of pages required, so that there are 5 Suber-Admin per page
            int pageSize = 5;
            int totalCustomers = allSuberAdmin.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            // Check if the requested page does not exist
            if (page > totalPages && totalPages > 0)
                return NotFound();

            // Split the Suber-Admin and display only 5 per page
            allSuberAdmin = allSuberAdmin.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the number of pages to the View
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;


            return View(allSuberAdmin.ToList());
        }
        #endregion

        #region New Super Admin

        // Display the new Super Admin creation form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // Process a new Super Admin creation request when the form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken] // Protection against CSRF attacks
        public async Task<IActionResult> Create(ApplicationUser applicationUser)
        {
            // Validate the entered data before saving
            if (ModelState.IsValid)
            {
                // Add the new Super Admin to the database
                _applicationUserRepository.Create(applicationUser);

                // Assign the "Super Admin" role to the new user
                await _userManager.AddToRoleAsync(applicationUser, "SuberAdmin");

                // Save changes to the database
                _applicationUserRepository.SaveDB();

                // Store a success message in TempData to display after redirection
                TempData["notifiction"] = "The Customer was created successfully!";
                TempData["MessageType"] = "success";

                // Redirect to the All Suber Admin view page
                return RedirectToAction(nameof(AllSuperAdmins));
            }

            // If there is an input error, re-render the form with the same data
            return View(applicationUser);
        }

        #endregion



    }
}

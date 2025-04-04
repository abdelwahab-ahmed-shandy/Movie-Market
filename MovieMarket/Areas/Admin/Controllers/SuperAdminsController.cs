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
        private readonly ILogger<AdminsController> _logger;

        public SuperAdminsController(IApplicationUserRepository applicationUserRepository,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdminsController> logger)
        {
            this._userManager = userManager;
            this._applicationUserRepository = applicationUserRepository;
            this._logger = logger;
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


        #region Block Customer Account :

        // This action is responsible for blocking a customer's account based on their ID
        public async Task<IActionResult> Block(string Id)
        {
            // Attempt to retrieve the user from the database using the provided ID
            var userDB = await _userManager.FindByIdAsync(Id);

            // Check if the user was found
            if (userDB != null)
            {
                // Set the 'IsBlocked' flag to true to block the user
                userDB.IsBlocked = true;

                // Update the user data in the database
                var result = await _userManager.UpdateAsync(userDB);

                // Check if the update was successful
                if (result.Succeeded)
                {
                    // Display a success message to the admin
                    TempData["Message"] = "The SuberAdmin's account has been successfully banned.";
                    TempData["MessageType"] = "Warning";

                    // Optional: Log the blocking action for auditing and security tracking
                    _logger.LogInformation($"User {userDB.Email} has been blocked.");
                }
                else
                {
                    // If update failed, show an error message
                    TempData["Message"] = "An error occurred while blocking the account.";
                    TempData["MessageType"] = "error";
                }

                // Redirect back to the list of all customers
                return RedirectToAction("AllSuperAdmins");
            }

            // If the user was not found, display an appropriate error message
            TempData["Message"] = "Client not found?!";
            TempData["MessageType"] = "error";

            // Redirect to the customers list even if the user wasn't found
            return RedirectToAction("AllSuperAdmins");
        }

        #endregion


        #region Un Block Customer Account :

        // This action is responsible for unblocking a customer's account based on their ID
        public async Task<IActionResult> UnBlock(string Id)
        {
            // Attempt to retrieve the user from the database using the provided ID
            var userDB = await _userManager.FindByIdAsync(Id);

            // Check if the user was found
            if (userDB != null)
            {
                // Set the 'IsBlocked' flag to false to unblock the user
                userDB.IsBlocked = false;

                // Update the user data in the database 
                var result = await _userManager.UpdateAsync(user: userDB);

                // Check if the update was successful
                if (result.Succeeded)
                {
                    // Display a success message to the admin
                    TempData["Message"] = "The SuberAdmins's account has been successfully unblocked.";
                    TempData["MessageType"] = "Success";

                    // Optional: Log the unblocking action for auditing and security tracking
                    _logger.LogInformation($"User {userDB.Email} has been unblocked.");
                }
                else
                {
                    // If update failed, show an error message
                    TempData["Message"] = "An error occurred while unblocking the account.";
                    TempData["MessageType"] = "error";
                }
                // Redirect back to the list of all customers
                return RedirectToAction("AllSuperAdmins");
            }
            // If the user was not found, display an appropriate error message
            TempData["Message"] = "Client not found?!";
            TempData["MessageType"] = "error";

            // Redirect to the customers list even if the user wasn't found
            return RedirectToAction("AllSuperAdmins");
        }

        #endregion


    }
}

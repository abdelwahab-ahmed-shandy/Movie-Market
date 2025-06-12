using DAL.ViewModels.Character;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CharacterController : Controller
    {
        private readonly IAdminCharacterService _characterService;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;

        public CharacterController(
            IAdminCharacterService characterService,
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<TvSeries> tvSeriesRepo)
        {
            _characterService = characterService;
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
        }


        #region Character Views

        // GET: Admin/Character
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var characters = await _characterService.GetCharacterAll(pageNumber, pageSize, searchTerm);
            return View(characters);
        }

        // GET: Admin/Character/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }

        #endregion



        #region Create 
        // GET: Admin/Character/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Movies = await _movieRepo.GetAll().ToListAsync();
            ViewBag.TvSeries = await _tvSeriesRepo.GetAll().ToListAsync();
            return View();
        }

        // POST: Admin/Character/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCharacterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _characterService.CreateCharacter(model);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error creating character");
            }

            ViewBag.Movies = await _movieRepo.GetAll().ToListAsync();
            ViewBag.TvSeries = await _tvSeriesRepo.GetAll().ToListAsync();
            return View(model);
        }

        #endregion



        #region Edit
        // GET: Admin/Character/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            var movies = await _movieRepo.GetAll().ToListAsync();
            var tvSeries = await _tvSeriesRepo.GetAll().ToListAsync();

            ViewBag.Movies = movies;
            ViewBag.TvSeries = tvSeries;

            var model = new EditCharacterVM
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description
            };

            return View(model);
        }

        // POST: Admin/Character/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCharacterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _characterService.UpdateCharacter(model);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error updating character");
            }

            ViewBag.Movies = await _movieRepo.GetAll().ToListAsync();
            ViewBag.TvSeries = await _tvSeriesRepo.GetAll().ToListAsync();
            return View(model);
        }

        #endregion



        #region Delete
        // POST: Admin/Character/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _characterService.DeleteCharacter(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        // POST: Admin/Character/SoftDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _characterService.SoftDeleteCharacter(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion



    }
}

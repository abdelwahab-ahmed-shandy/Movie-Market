using DAL.ViewModels.Character;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class CharacterController : Controller
    {
        private readonly IAdminCharacterService _characterService;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IAdminCharacterTvSeriesService _characterTvSeriesService;

        public CharacterController(IAdminCharacterService characterService,IGenericRepository<Movie> movieRepo,
                                        IGenericRepository<TvSeries> tvSeriesRepo , IAdminCharacterTvSeriesService characterTvSeriesService)
        {
            _characterService = characterService;
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _characterTvSeriesService = characterTvSeriesService;
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
                    TempData["notification"] = "Character created successfully!";
                    TempData["MessageType"] = "success";
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
                    TempData["notification"] = "Character Is Updated !";
                    TempData["MessageType"] = "Success";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error updating character");
            }

            ViewBag.Movies = await _movieRepo.GetAll().ToListAsync();
            ViewBag.TvSeries = await _tvSeriesRepo.GetAll().ToListAsync();
            return View(model);
        }

        #endregion



        #region Add To TvSeries

        [HttpGet]
        public async Task<IActionResult> AddToTvSeries(Guid tvSeriesId)
        {
            try
            {
                var model = await _characterTvSeriesService.GetAddCharactersViewModel(tvSeriesId);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToTvSeries(CharacterTvSeriesAddVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _characterTvSeriesService.AddCharacterToTvSeries(model.TvSeriesId, model.CharacterId);
                    TempData["SuccessMessage"] = "Character added successfully!";
                    return RedirectToAction("Details", "TvSeries", new { id = model.TvSeriesId });
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // If we got this far, something failed; reload the available characters
            model.AvailableCharacters = (await _characterTvSeriesService.GetAddCharactersViewModel(model.TvSeriesId)).AvailableCharacters;
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
            TempData["notification"] = "Character permanently deleted!";
            TempData["MessageType"] = "Warning";
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

            TempData["notification"] = "Character Change Status Soft Deleted!";
            TempData["MessageType"] = "Information";
            return RedirectToAction(nameof(Index));
        }
        #endregion


    }
}

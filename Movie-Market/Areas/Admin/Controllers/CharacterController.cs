using DAL.ViewModels.Character;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CharacterController : BaseController
    {
        private readonly ICharacterService _characterService;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly ICharacterTvSeriesService _characterTvSeriesService;

        public CharacterController(ICharacterService characterService,IGenericRepository<Movie> movieRepo,
                                        IGenericRepository<TvSeries> tvSeriesRepo , ICharacterTvSeriesService characterTvSeriesService
                                            , UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _characterService = characterService;
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _characterTvSeriesService = characterTvSeriesService;
        }


        #region Character Views

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var characters = await _characterService.GetCharacterAllAsync(pageNumber, pageSize, searchTerm);
            return View(characters);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var character = await _characterService.GetCharacterByIdAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }

        #endregion


        #region Create 
        public async Task<IActionResult> Create()
        {
            ViewBag.Movies = await _movieRepo.GetAll().ToListAsync();
            ViewBag.TvSeries = await _tvSeriesRepo.GetAll().ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCharacterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _characterService.CreateCharacterAsync(model);
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
        public async Task<IActionResult> Edit(Guid id)
        {
            var character = await _characterService.GetCharacterByIdAsync(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCharacterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _characterService.UpdateCharacterAsync(model);
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
                var model = await _characterTvSeriesService.GetAddCharactersViewModelAsync(tvSeriesId);
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
                    await _characterTvSeriesService.AddCharacterToTvSeriesAsync(model.TvSeriesId, model.CharacterId);

                    TempData["notification"] = "Character added successfully!";
                    TempData["MessageType"] = "success";

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

            model.AvailableCharacters = (await _characterTvSeriesService.GetAddCharactersViewModelAsync(model.TvSeriesId)).AvailableCharacters;
            return View(model);
        }

        #endregion


        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _characterService.DeleteCharacterAsync(id);
            if (!result)
            {
                return NotFound();
            }
            TempData["notification"] = "Character permanently deleted!";
            TempData["MessageType"] = "Warning";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _characterService.SoftDeleteCharacterAsync(id);
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

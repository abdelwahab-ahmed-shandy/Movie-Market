using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class CharacterController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITvSeriesRepository _tvSeriesRepository;
        public CharacterController(ICharacterRepository characterRepository, IMovieRepository movieRepository, ITvSeriesRepository tvSeriesRepository)
        {
            this._characterRepository = characterRepository;
            this._tvSeriesRepository = tvSeriesRepository;
            this._movieRepository = movieRepository;
        }
        public IActionResult Index(string? query, int page = 1)
        {
            var characters = _characterRepository.Get().ToList();

            if (query != null)
            {
                characters = characters.Where(e => (e.Name ?? "").Contains(query) ||
                                                    (e.Description ?? "").Contains(query)
                                                    ).ToList();
            }

            int pageSize = 5;
            int totalCharacters = characters.Count();
            int totalPages = (int)Math.Ceiling((double)totalCharacters / pageSize);

            if (page > totalPages && totalPages > 0)
                return NotFound();

            characters = characters.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            return View(characters.ToList());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new Character());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Character character)
        {
            if (ModelState.IsValid)
            {
                _characterRepository.Create(character);
                _characterRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "The Character was created successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            return View(character);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var character = _characterRepository.GetOne(c => c.Id == Id);
            if (character != null)
            {
                return View(character);
            }
            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Character character)
        {
            if (ModelState.IsValid)
            {
                _characterRepository.Edit(character);
                _characterRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "Edit Character Successfully!";
                TempData["MessageType"] = "Success";
                return RedirectToAction(nameof(Index));
            }
            return View(character);
        }

        public IActionResult Delete(int Id)
        {
            var character = _characterRepository.GetOne(c => c.Id == Id);

            if (character == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _characterRepository.Delete(character);
            _characterRepository.SaveDB();

            // Set the success message in TempData
            TempData["notifiction"] = "Character Deleted Successfully!";
            TempData["MessageType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
    }
}

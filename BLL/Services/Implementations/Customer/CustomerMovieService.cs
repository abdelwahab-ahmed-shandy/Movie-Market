using BLL.Services.Interfaces.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CustomerMovieService : ICustomerMovieService
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CinemaMovie> _cinemaMovieRepo;
        private readonly IGenericRepository<MovieSpecial> _movieSpecialRepo;

        public CustomerMovieService(
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<CharacterMovie> characterMovieRepo,
            IGenericRepository<CinemaMovie> cinemaMovieRepo,
            IGenericRepository<MovieSpecial> movieSpecialRepo)
        {
            _movieRepo = movieRepo;
            _categoryRepo = categoryRepo;
            _characterMovieRepo = characterMovieRepo;
            _cinemaMovieRepo = cinemaMovieRepo;
            _movieSpecialRepo = movieSpecialRepo;
        }

        public async Task<IEnumerable<MovieIndexVM>> GetActiveMoviesAsync()
        {
            var now = DateTime.UtcNow;

            var movies = await _movieRepo.Get(m => m.CurrentState.Value == DAL.Enums.CurrentState.Active && !m.IsDeleted &&
                (m.StartDate == null || m.StartDate <= now) &&
                (m.EndDate == null || m.EndDate >= now))
                .Include(m => m.Category)
                .OrderByDescending(m => m.Rating)
                .ToListAsync();

            return movies.Select(m => new MovieIndexVM
            {
                Id = m.Id,
                Title = m.Title,
                ImgUrl = m.ImgUrl,
                Price = m.Price,
                Rating = m.Rating,
                CategoryName = m.Category.Name
            });
        }

        public async Task<List<MovieDetailsVM>> GetMoviesNewReleasesAsync(int count)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-3);

            var newReleases = await _movieRepo.Get(m =>
                    m.CurrentState.Value == DAL.Enums.CurrentState.Active &&
                    !m.IsDeleted &&
                    m.CreatedDateUtc >= cutoffDate)
                .OrderByDescending(m => m.StartDate)
                .ThenByDescending(m => m.Rating)
                .Take(count)
                .Include(m => m.Category)
                .Select(m => new MovieDetailsVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    ImgUrl = m.ImgUrl,
                    Price = m.Price,
                    Rating = m.Rating,
                    CategoryName = m.Category != null ? m.Category.Name : string.Empty
                })
                .ToListAsync();

            return newReleases;
        }

        public async Task<MovieDetailsVM?> GetMovieDetailsAsync(Guid id)
        {
            var movie = await _movieRepo.Get(m => m.Id == id && !m.IsDeleted)
                .Include(m => m.Category)
                .Include(m => m.CharacterMovies).ThenInclude(cm => cm.Character)
                .Include(m => m.CinemaMovies).ThenInclude(cm => cm.Cinema)
                .Include(m => m.MovieSpecials).ThenInclude(ms => ms.Special)
                .FirstOrDefaultAsync();

            if (movie == null) return null;

            return new MovieDetailsVM
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Price = movie.Price,
                Author = movie.Author,
                ImgUrl = movie.ImgUrl,
                Duration = movie.Duration,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                ReleaseYear = movie.ReleaseYear,
                Rating = movie.Rating,
                CategoryName = movie.Category.Name,
                Characters = movie.CharacterMovies.Select(cm => cm.Character.Name),
                Cinemas = movie.CinemaMovies.Select(cm => cm.Cinema.Name),
                Specials = movie.MovieSpecials.Select(ms => ms.Special.Name)
            };
        }

        public async Task<IEnumerable<MovieIndexVM>> GetPopularMoviesAsync(int count)
        {
            var movies = await _movieRepo.GetAll()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Rating)
                .Take(count)
                .Include(m => m.Category)
                .ToListAsync();

            return movies.Select(m => new MovieIndexVM
            {
                Id = m.Id,
                Title = m.Title,
                ImgUrl = m.ImgUrl,
                Price = m.Price,
                Rating = m.Rating,
                CategoryName = m.Category.Name
            });
        }

        public async Task<IEnumerable<MovieIndexVM>> GetMoviesByCategoryAsync(Guid categoryId)
        {
            var movies = await _movieRepo.Get(m =>
                !m.IsDeleted &&
                m.CategoryId == categoryId)
                .Include(m => m.Category)
                .ToListAsync();

            return movies.Select(m => new MovieIndexVM
            {
                Id = m.Id,
                Title = m.Title,
                ImgUrl = m.ImgUrl,
                Price = m.Price,
                Rating = m.Rating,
                CategoryName = m.Category.Name
            });
        }


    }
}

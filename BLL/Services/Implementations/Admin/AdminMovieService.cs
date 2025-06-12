using BLL.Services.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class AdminMovieService : IAdminMovieService
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CinemaMovie> _cinemaMovieRepo;
        private readonly IGenericRepository<MovieSpecial> _movieSpecialRepo;

        public AdminMovieService(
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Special> specialRepo,
            IGenericRepository<CharacterMovie> characterMovieRepo,
            IGenericRepository<CinemaMovie> cinemaMovieRepo,
            IGenericRepository<MovieSpecial> movieSpecialRepo)
        {
            _movieRepo = movieRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _cinemaRepo = cinemaRepo;
            _specialRepo = specialRepo;
            _characterMovieRepo = characterMovieRepo;
            _cinemaMovieRepo = cinemaMovieRepo;
            _movieSpecialRepo = movieSpecialRepo;
        }



        public async Task<MovieAdminListResultVM> GetAllMoviesAsync(int page, int pageSize, string? query = null)
        {
            // Start with Include to maintain IIncludableQueryable type
            var source = _movieRepo.GetAllWithDeleted()
                .Include(m => m.Category)
                .AsQueryable();  // Explicitly cast to IQueryable if needed

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(m =>
                    m.Title.Contains(query) ||
                    (m.Category != null && m.Category.Name.Contains(query)));
            }

            var paginatedList = await PaginatedList<Movie>.CreateAsync(
                source.OrderBy(m => m.Title),
                page,
                pageSize);

            return new MovieAdminListResultVM
            {
                Movies = paginatedList.Select(m => new MovieAdminListVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    ImgUrl = m.ImgUrl,
                    CurrentState = m.CurrentState.Value,
                    Price = m.Price,
                    Rating = m.Rating,
                    CategoryName = m.Category != null ? m.Category.Name : string.Empty,
                    IsDeleted = m.IsDeleted
                }),
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                TotalCount = paginatedList.TotalCount,
                SearchTerm = query
            };
        }



        public async Task<MovieAdminEditVM?> GetMovieForEditAsync(Guid id)
        {
            var movie = await _movieRepo.Get(m => m.Id == id)
                .Include(m => m.CharacterMovies)
                .Include(m => m.CinemaMovies)
                .Include(m => m.MovieSpecials)
                .FirstOrDefaultAsync();

            if (movie == null) return null;

            return new MovieAdminEditVM
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                CurrentState = movie.CurrentState.Value,
                Price = movie.Price,
                Author = movie.Author,
                ImgUrl = movie.ImgUrl,
                Duration = movie.Duration,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                ReleaseYear = movie.ReleaseYear,
                Rating = movie.Rating,
                CategoryId = movie.CategoryId,
                CharacterIds = movie.CharacterMovies.Select(cm => cm.CharacterId).ToList(),
                CinemaIds = movie.CinemaMovies.Select(cm => cm.CinemaId).ToList(),
                SpecialIds = movie.MovieSpecials.Select(ms => ms.SpecialId).ToList()
            };
        }


        public async Task<bool> CreateMovieAsync(MovieAdminCreateVM model)
        {
            try
            {
                var movie = new Movie
                {
                    Title = model.Title,
                    Description = model.Description,
                    CurrentState = model.CurrentState,
                    Price = model.Price,
                    Author = model.Author,
                    ImgUrl = model.ImgUrl,
                    Duration = model.Duration,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ReleaseYear = model.ReleaseYear,
                    Rating = model.Rating,
                    CategoryId = model.CategoryId
                };

                await _movieRepo.Add(movie);

                // Add character relationships
                foreach (var characterId in model.CharacterIds)
                {
                    await _characterMovieRepo.Add(new CharacterMovie
                    {
                        MovieId = movie.Id,
                        CharacterId = characterId
                    });
                }

                // Add cinema relationships
                foreach (var cinemaId in model.CinemaIds)
                {
                    await _cinemaMovieRepo.Add(new CinemaMovie
                    {
                        MovieId = movie.Id,
                        CinemaId = cinemaId
                    });

                }

                // Add special relationships
                foreach (var specialId in model.SpecialIds)
                {
                    await _movieSpecialRepo.Add(new MovieSpecial
                    {
                        ParentMovieId = movie.Id,
                        SpecialId = specialId
                    });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> UpdateMovieAsync(MovieAdminEditVM model)
        {
            try
            {
                var movie = await _movieRepo.GetByIdAsync(model.Id);
                if (movie == null) return false;

                // Update movie properties
                movie.Title = model.Title;
                movie.Description = model.Description;
                movie.Price = model.Price;
                movie.Author = model.Author;
                movie.ImgUrl = model.ImgUrl;
                movie.Duration = model.Duration;
                movie.StartDate = model.StartDate;
                movie.EndDate = model.EndDate;
                movie.ReleaseYear = model.ReleaseYear;
                movie.Rating = model.Rating;
                movie.CurrentState = model.CurrentState;
                movie.CategoryId = model.CategoryId;

                await _movieRepo.Update(movie);

                // Update character relationships
                var existingCharacterMovies = await _characterMovieRepo.Get(cm => cm.MovieId == movie.Id).ToListAsync();
                foreach (var existing in existingCharacterMovies)
                {
                    await _characterMovieRepo.DeleteInDB(existing.Id);
                }

                foreach (var characterId in model.CharacterIds)
                {
                    await _characterMovieRepo.Add(new CharacterMovie
                    {
                        MovieId = movie.Id,
                        CharacterId = characterId
                    });
                }

                // Update cinema relationships
                var existingCinemaMovies = await _cinemaMovieRepo.Get(cm => cm.MovieId == movie.Id).ToListAsync();
                foreach (var existing in existingCinemaMovies)
                {
                    await _cinemaMovieRepo.DeleteInDB(existing.Id);
                }

                foreach (var cinemaId in model.CinemaIds)
                {
                    await _cinemaMovieRepo.Add(new CinemaMovie
                    {
                        MovieId = movie.Id,
                        CinemaId = cinemaId,
                        ShowTime = model.CinemaShowTimes.TryGetValue(cinemaId, out var showTime) ? showTime : DateTime.UtcNow
                    });
                }

                // Update special relationships
                var existingMovieSpecials = await _movieSpecialRepo.Get(ms => ms.ParentMovieId == movie.Id).ToListAsync();
                foreach (var existing in existingMovieSpecials)
                {
                    await _movieSpecialRepo.DeleteInDB(existing.Id);
                }

                foreach (var specialId in model.SpecialIds)
                {
                    await _movieSpecialRepo.Add(new MovieSpecial
                    {
                        ParentMovieId = movie.Id,
                        SpecialId = specialId,
                        IsFeatured = model.SpecialFeatures.TryGetValue(specialId, out var featured) && featured,
                        DisplayOrder = model.SpecialDisplayOrders.TryGetValue(specialId, out var order) ? order : 0
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating movie: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SoftDeleteMovieAsync(Guid id)
        {
            try
            {
                await _movieRepo.SoftDeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> RestoreMovieAsync(Guid id)
        {
            try
            {
                await _movieRepo.RestoreAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> DeleteMoviePermanentlyAsync(Guid id)
        {
            try
            {
                await _movieRepo.DeleteInDB(id);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}

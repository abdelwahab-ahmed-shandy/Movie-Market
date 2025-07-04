
namespace BLL.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CinemaMovie> _cinemaMovieRepo;
        private readonly IGenericRepository<MovieSpecial> _movieSpecialRepo;
        private readonly IFileService _fileService;
        private readonly ILogger<MovieService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MovieService(IGenericRepository<Movie> movieRepo,IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Special> specialRepo,IGenericRepository<CharacterMovie> characterMovieRepo,
            IGenericRepository<CinemaMovie> cinemaMovieRepo,IGenericRepository<MovieSpecial> movieSpecialRepo,
            ILogger<MovieService> logger, IFileService fileService , IHttpContextAccessor httpContextAccessor)
        {
            _movieRepo = movieRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _cinemaRepo = cinemaRepo;
            _specialRepo = specialRepo;
            _characterMovieRepo = characterMovieRepo;
            _cinemaMovieRepo = cinemaMovieRepo;
            _movieSpecialRepo = movieSpecialRepo;
            _logger = logger;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }


        #region Admin Methodes

        public async Task<MovieAdminListResultVM> GetAllMoviesAsync(int page, int pageSize, string? query = null)
        {
            var source = _movieRepo.GetAllWithDeleted()
                .Include(m => m.Category)
                .AsQueryable();  

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
    
        public async Task<MovieAdminDetailsVM?> GetMovieDetailsAsync(Guid id)
        {
            var movie = await _movieRepo.GetAll()
                .Include(m => m.Category)
                .Include(m => m.CharacterMovies)
                    .ThenInclude(cm => cm.Character)
                .Include(m => m.CinemaMovies)
                    .ThenInclude(cm => cm.Cinema)
                .Include(m => m.MovieSpecials)
                    .ThenInclude(ms => ms.Special)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return null;

            return new MovieAdminDetailsVM
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
                Category = movie.Category != null ? new CategoryMovieVM
                {
                    Id = movie.Category.Id,
                    Name = movie.Category.Name
                } : null,
                Characters = movie.CharacterMovies
                    .Where(cm => cm.Character != null)
                    .Select(cm => new CharacterMovieVM
                    {
                        Id = cm.Character.Id,
                        Name = cm.Character.Name,
                        Description = cm.Character.Description
                    }).ToList(),
                Cinemas = movie.CinemaMovies
                    .Where(cm => cm.Cinema != null)
                    .Select(cm => new CinemaMovieVM
                    {
                        Id = cm.Cinema.Id,
                        Name = cm.Cinema.Name,
                        Location = cm.Cinema.Location,
                        ShowTime = cm.ShowTime
                    }).ToList(),
                Specials = movie.MovieSpecials
                    .Where(ms => ms.Special != null)
                    .Select(ms => new SpecialMovieViewModel
                    {
                        Id = ms.Special.Id,
                        Title = ms.Special.Name,
                        Description = ms.Special.Description,
                        IsFeatured = ms.IsFeatured,
                        DisplayOrder = ms.DisplayOrder
                    }).ToList(),
                CreatedBy = movie.CreatedBy,
                CreatedDateUtc = movie.CreatedDateUtc,
                UpdatedBy = movie.UpdatedBy,
                UpdatedDateUtc = movie.UpdatedDateUtc,
                DeletedBy = movie.DeletedBy,
                DeletedDateUtc = movie.DeletedDateUtc,
                IsDeleted = movie.IsDeleted
            };
        }
        
        public async Task<bool> CreateMovieAsync(MovieAdminCreateVM model)
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

                string? imagePath = null;
                if (model.ImgFile != null && model.ImgFile.Length > 0)
                {
                    imagePath = await _fileService.SaveFileAsync(model.ImgFile, "uploads/movies");
                }

                var movie = new Movie
                {
                    Title = model.Title,
                    Description = model.Description,
                    CurrentState = model.CurrentState,
                    Price = model.Price,
                    Author = model.Author,
                    ImgUrl = imagePath,
                    Duration = model.Duration,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ReleaseYear = model.ReleaseYear,
                    Rating = model.Rating,
                    CreatedBy = userName,
                    CreatedDateUtc = DateTime.UtcNow,
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

        public async Task<bool> UpdateMovieAsync(MovieAdminEditVM model)
        {
            try
            {
                var movie = await _movieRepo.GetByIdAsync(model.Id);
                if (movie == null)
                {
                    _logger.LogWarning("Movie with ID {MovieId} not found for update", model.Id);
                    return false;
                }

                var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

                if (model.ImgFile != null && model.ImgFile.Length > 0)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(movie.ImgUrl))
                        {
                            _fileService.DeleteFile(movie.ImgUrl);
                        }
                        movie.ImgUrl = await _fileService.SaveFileAsync(model.ImgFile, "uploads/movies");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating image for movie {MovieId}", movie.Id);
                        throw; 
                    }
                }

                UpdateMovieProperties(movie, model, userName);
                await _movieRepo.Update(movie);

                await UpdateCharacterRelationships(movie.Id, model.CharacterIds ?? new List<Guid>());
                await UpdateCinemaRelationships(movie.Id, model.CinemaIds ?? new List<Guid>());
                await UpdateSpecialRelationships(movie.Id, model.SpecialIds ?? new List<Guid>());

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with ID {MovieId}", model.Id);
                return false;
            }
        }

        #region Update Movie Properties
        private void UpdateMovieProperties(Movie movie, MovieAdminEditVM model, string userName)
        {
            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.Price = model.Price;
            movie.Author = model.Author;
            movie.Duration = model.Duration;
            movie.StartDate = model.StartDate;
            movie.EndDate = model.EndDate;
            movie.ReleaseYear = model.ReleaseYear;
            movie.Rating = model.Rating;
            movie.CurrentState = model.CurrentState;
            movie.CategoryId = model.CategoryId;
            movie.UpdatedBy = userName;
            movie.UpdatedDateUtc = DateTime.UtcNow;
        }

        private async Task UpdateCharacterRelationships(Guid movieId, List<Guid> newCharacterIds)
        {
            var existing = await _characterMovieRepo.Get(cm => cm.MovieId == movieId).ToListAsync();
            var existingIds = existing.Select(cm => cm.CharacterId).ToList();

            // حذف العلاقات غير الموجودة في الجديدة
            foreach (var item in existing.Where(cm => !newCharacterIds.Contains(cm.CharacterId)))
            {
                await _characterMovieRepo.DeleteInDB(item.Id);
            }

            // إضافة العلاقات الجديدة
            foreach (var characterId in newCharacterIds.Except(existingIds))
            {
                await _characterMovieRepo.Add(new CharacterMovie { MovieId = movieId, CharacterId = characterId });
            }
        }

        private async Task UpdateCinemaRelationships(Guid movieId, List<Guid> newCinemaIds)
        {
            var existing = await _cinemaMovieRepo.Get(cm => cm.MovieId == movieId).ToListAsync();
            var existingIds = existing.Select(cm => cm.CinemaId).ToList();

            foreach (var item in existing.Where(cm => !newCinemaIds.Contains(cm.CinemaId)))
            {
                await _cinemaMovieRepo.DeleteInDB(item.Id);
            }

            foreach (var cinemaId in newCinemaIds.Except(existingIds))
            {
                await _cinemaMovieRepo.Add(new CinemaMovie { MovieId = movieId, CinemaId = cinemaId });
            }
        }

        private async Task UpdateSpecialRelationships(Guid movieId, List<Guid> newSpecialIds)
        {
            var existing = await _movieSpecialRepo.Get(ms => ms.ParentMovieId == movieId).ToListAsync();
            var existingIds = existing.Select(ms => ms.SpecialId).ToList();

            foreach (var item in existing.Where(ms => !newSpecialIds.Contains(ms.SpecialId)))
            {
                await _movieSpecialRepo.DeleteInDB(item.Id);
            }

            foreach (var specialId in newSpecialIds.Except(existingIds))
            {
                await _movieSpecialRepo.Add(new MovieSpecial { ParentMovieId = movieId, SpecialId = specialId });
            }
        }
        #endregion

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
                var movie = await _movieRepo.GetByIdAsync(id);
                if (movie == null) return false;

                if (!string.IsNullOrWhiteSpace(movie.ImgUrl))
                {
                    _fileService.DeleteFile(movie.ImgUrl);
                }

                await _movieRepo.DeleteInDB(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting movie: {ex.Message}");
                return false;
            }
        }

        #endregion


        #region Customer Methods

        public async Task<IEnumerable<MovieCharacterIndexVM>> GetActiveMoviesAsync()
        {
            var now = DateTime.UtcNow;

            var movies = await _movieRepo.Get(m => m.CurrentState.Value == DAL.Enums.CurrentState.Active && !m.IsDeleted &&
                (m.StartDate == null || m.StartDate <= now) &&
                (m.EndDate == null || m.EndDate >= now))
                .Include(m => m.Category)
                .OrderByDescending(m => m.Rating)
                .ToListAsync();

            return movies.Select(m => new MovieCharacterIndexVM
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

        public async Task<MovieDetailsVM?> GetMovieWithDetailsAsync(Guid id)
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
                Specials = movie.MovieSpecials.Select(ms => ms.Special.Name),
                Cinemas = movie.CinemaMovies.Select(cm => new CinemaMovieDetailsVM
                {
                    Id = cm.Cinema.Id,
                    Name = cm.Cinema.Name,
                    Location = cm.Cinema.Location
                })
            };
        }

        public async Task<IEnumerable<MovieCharacterIndexVM>> GetPopularMoviesAsync(int count)
        {
            var movies = await _movieRepo.GetAll()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Rating)
                .Take(count)
                .Include(m => m.Category)
                .ToListAsync();

            return movies.Select(m => new MovieCharacterIndexVM
            {
                Id = m.Id,
                Title = m.Title,
                ImgUrl = m.ImgUrl,
                Price = m.Price,
                Rating = m.Rating,
                CategoryName = m.Category.Name
            });
        }

        public async Task<IEnumerable<MovieCharacterIndexVM>> GetMoviesByCategoryAsync(Guid categoryId)
        {
            var movies = await _movieRepo.Get(m =>
                !m.IsDeleted &&
                m.CategoryId == categoryId)
                .Include(m => m.Category)
                .ToListAsync();

            return movies.Select(m => new MovieCharacterIndexVM
            {
                Id = m.Id,
                Title = m.Title,
                ImgUrl = m.ImgUrl,
                Price = m.Price,
                Rating = m.Rating,
                CategoryName = m.Category.Name
            });
        }

        #endregion

    }
}

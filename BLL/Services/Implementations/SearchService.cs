
namespace BLL.Services.Implementations
{
    public class SearchService : ISearchService
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Season> _seasonRepo;
        private readonly IGenericRepository<Episode> _episodeRepo;

        public SearchService(IGenericRepository<Movie> movieRepo, IGenericRepository<Category> categoryRepo,
                                IGenericRepository<TvSeries> tvSeriesRepo,
                                    IGenericRepository<Character> characterRepo,
                                        IGenericRepository<Cinema> cinemaRepo,
                                            IGenericRepository<Season> seasonRepo,
                                                IGenericRepository<Episode> episodeRepo)
        {
            _movieRepo = movieRepo;
            _categoryRepo = categoryRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _characterRepo = characterRepo;
            _cinemaRepo = cinemaRepo;
            _seasonRepo = seasonRepo;
            _episodeRepo = episodeRepo;
        }


        #region Admin
        public async Task<SearchAdminResultVM> GlobalSearchAdminAsync(string query)
        {
            var result = new SearchAdminResultVM();

            if (string.IsNullOrWhiteSpace(query))
                return result;

            query = query.ToLower().Trim();

            // Search Movies (including deleted ones for admin)
            var movies = await _movieRepo.GetAllWithDeleted()
                .Where(m => m.Title.ToLower().Contains(query) ||
                           m.Description.ToLower().Contains(query) ||
                           m.Author.ToLower().Contains(query))
                .ToListAsync();

            result.Movies = movies.Select(m => new MovieAdminSearchResultVM
            {
                Title = m.Title,
                Author = m.Author,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                Price = m.Price,
                Bases = new List<BaseAdminModel> { GetBaseAdminModel(m) }
            }).ToList();


            // Search Cinemas
            result.Cinemas = await _cinemaRepo.GetAllWithDeleted()
                .Where(c => c.Name.ToLower().Contains(query) ||
                           c.Description.ToLower().Contains(query) ||
                           c.Location.ToLower().Contains(query))
                .Select(c => new CinemaAdminSearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description,
                    Location = c.Location,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(c) }
                })
                .ToListAsync();


            // Search Categories
            result.Categories = await _categoryRepo.GetAllWithDeleted()
                .Where(c => c.Name.ToLower().Contains(query) ||
                           c.Description.ToLower().Contains(query))
                .Select(c => new CategoryAdminSearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(c) }
                }).ToListAsync();


            // Seasons 
            result.Seasons = await _seasonRepo.GetAllWithDeleted()
                .Where(s => s.Title.ToLower().Contains(query))
                .Select(s => new SeasonAdminSearchResultVM
                {
                    Title = s.Title,
                    SeasonNumber = s.SeasonNumber,
                    ReleaseYear = s.ReleaseYear,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(s) }
                }).ToListAsync();


            // Episodes
            result.Episodes = await _episodeRepo.GetAllWithDeleted()
                .Where(e => e.Title.ToLower().Contains(query) ||
                           e.Description.ToLower().Contains(query))
                .Select(e => new EpisodeAdminSearchResultVM
                {
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    Duration = e.Duration,
                    Description = e.Description,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(e) }
                }).ToListAsync();


            // Characters
            result.Characters = await _characterRepo.GetAllWithDeleted()
                .Where(c => c.Name.ToLower().Contains(query) ||
                           c.Description.ToLower().Contains(query))
                .Select(c => new CharacterAdminSearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(c) }
                }).ToListAsync();


            // Tv series
            result.TvSeries = await _tvSeriesRepo.GetAllWithDeleted()
                .Where(t => t.Title.ToLower().Contains(query) ||
                           t.Description.ToLower().Contains(query) ||
                           t.Author.ToLower().Contains(query))
                .Select(t => new TvSeriesAdminSearchResultVM
                {
                    Title = t.Title,
                    Description = t.Description,
                    Author = t.Author,
                    ReleaseYear = t.ReleaseYear,
                    Bases = new List<BaseAdminModel> { GetBaseAdminModel(t) }
                }).ToListAsync();


            result.TotalResults = result.Movies.Count + result.Cinemas.Count
                                    + result.Categories.Count + result.Characters.Count
                                    + result.Episodes.Count + result.Seasons.Count
                                    + result.TvSeries.Count;

            return result;
        }

        private static BaseAdminModel GetBaseAdminModel(BaseModel entity)
        {
            return new BaseAdminModel
            {
                Id = entity.Id,
                CurrentState = entity.CurrentState,
                CreatedBy = entity.CreatedBy,
                CreatedDateUtc = entity.CreatedDateUtc,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDateUtc = entity.UpdatedDateUtc,
                BlockedBy = entity.BlockedBy,
                BlockedDateUtc = entity.BlockedDateUtc,
                DeletedBy = entity.DeletedBy,
                DeletedDateUtc = entity.DeletedDateUtc,
                RestoredBy = entity.RestoredBy,
                RestoredDateUtc = entity.RestoredDateUtc
            };
        }

        #endregion


        #region Customer

        public async Task<SearchCutomerResultVM> GlobalSearchCustomerAsync(string query)
        {
            var result = new SearchCutomerResultVM();

            if (string.IsNullOrWhiteSpace(query))
                return result;

            query = query.ToLower().Trim();

            // Search Movies
            result.Movies = await _movieRepo.Get(m =>
                !m.IsDeleted &&
                (m.Title.ToLower().Contains(query) ||
                 m.Description.ToLower().Contains(query) ||
                 m.Author.ToLower().Contains(query)))
                .Select(m => new MovieSearchResultVM
                {
                    Title = m.Title,
                    Author = m.Author,
                    Description = m.Description,
                    ReleaseYear = m.ReleaseYear
                })
                .ToListAsync();

            // Search Cinemas
            result.Cinemas = await _cinemaRepo.Get(c =>
                !c.IsDeleted &&
                (c.Name.ToLower().Contains(query) ||
                 c.Description.ToLower().Contains(query) ||
                 c.Location.ToLower().Contains(query)))
                .Select(c => new CinemaSearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description,
                    Location = c.Location
                })
                .ToListAsync();

            // Search Categories
            result.Categories = await _categoryRepo.Get(c =>
                !c.IsDeleted &&
                (c.Name.ToLower().Contains(query) ||
                 c.Description.ToLower().Contains(query)))
                .Select(c => new CategorySearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            // Search TV Series
            result.TvSeries = await _tvSeriesRepo.Get(t =>
                !t.IsDeleted &&
                (t.Title.ToLower().Contains(query) ||
                 t.Description.ToLower().Contains(query) ||
                 t.Author.ToLower().Contains(query)))
                .Select(t => new TvSeriesSearchResultVM
                {
                    Title = t.Title,
                    Description = t.Description,
                    Author = t.Author,
                    ReleaseYear = t.ReleaseYear
                })
                .ToListAsync();

            // Search Seasons
            result.Seasons = await _seasonRepo.Get(s =>
                !s.IsDeleted &&
                (s.Title.ToLower().Contains(query)))
                .Select(s => new SeasonSearchResultVM
                {
                    Title = s.Title,
                    SeasonNumber = s.SeasonNumber,
                    ReleaseYear = s.ReleaseYear
                })
                .ToListAsync();

            // Search Episodes
            result.Episodes = await _episodeRepo.Get(e =>
                !e.IsDeleted &&
                (e.Title.ToLower().Contains(query) ||
                 e.Description.ToLower().Contains(query)))
                .Select(e => new EpisodeSearchResultVM
                {
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    Duration = e.Duration,
                    Description = e.Description
                })
                .ToListAsync();

            // Search Characters
            result.Characters = await _characterRepo.Get(c =>
                !c.IsDeleted &&
                (c.Name.ToLower().Contains(query) ||
                 c.Description.ToLower().Contains(query)))
                .Select(c => new CharacterSearchResultVM
                {
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            result.TotalResults = result.Movies.Count + result.Cinemas.Count + result.Categories.Count +
                                 result.TvSeries.Count + result.Seasons.Count + result.Episodes.Count +
                                 result.Characters.Count;

            return result;
        }


        #endregion


    }
}

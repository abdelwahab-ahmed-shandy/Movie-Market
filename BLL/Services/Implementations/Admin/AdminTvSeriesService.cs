using BLL.Services.Interfaces.Admin;
using DAL.ViewModels.TvSeries;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminTvSeriesService : IAdminTvSeriesService
    {
        private readonly IGenericRepository<TvSeries> _tvSeriesRepository;
        private readonly IGenericRepository<Season> _seasonRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public AdminTvSeriesService(IGenericRepository<TvSeries> tvSeriesRepo, IHttpContextAccessor HttpContextAccessor,
                                         IGenericRepository<Season> seasonRepo , IGenericRepository<Character> characterRepo)
        {
            _HttpContextAccessor = HttpContextAccessor;
            _tvSeriesRepository = tvSeriesRepo;
            _seasonRepo = seasonRepo;
            _characterRepo = characterRepo;
        }


        public async Task<TvSeriesAdminListVM> GetAllTvSeriesAsync(int page, int pageSize, string? query = null)
        {
            var tvSeriesQuery = _tvSeriesRepository.GetAllWithDeleted()
                                        .Include(t => t.Seasons.Where(s => !s.IsDeleted)).AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                tvSeriesQuery = tvSeriesQuery.Where(t => t.Title.Contains(query) || t.Description.Contains(query) || t.Author.Contains(query));
            }

            var totalCount = await tvSeriesQuery.CountAsync();

            var paginatedTvSeries = await PaginatedList<TvSeries>.CreateAsync(tvSeriesQuery.OrderBy(t => t.Title),page,pageSize);

            var tvSeriesVMs = paginatedTvSeries.Select(t => new TvSeriesAdminVM
            {
                Id = t.Id,
                Title = t.Title,
                ImgUrl = t.ImgUrl,
                ReleaseYear = t.ReleaseYear,
                Rating = t.Rating,
                SeasonCount = t.Seasons.Count,
                IsDeleted = t.IsDeleted,
                CurrentState = t.CurrentState.Value
            }).ToList();

            return new TvSeriesAdminListVM
            {
                TvSeries = tvSeriesVMs,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = query
            };
        }


        public async Task<TvSeriesAdminDetailsVM> GetTvSeriesDetailsAsync(Guid id)
        {
            var tvSeries = await _tvSeriesRepository.GetAllWithDeleted()
                .Include(t => t.Seasons)
                    .ThenInclude(s => s.Episodes)
                .Include(t => t.Characters)
                    .ThenInclude(c => c.Character)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tvSeries == null)
            {
                throw new KeyNotFoundException("TV Series not found");
            }

            var seasonsVM = tvSeries.Seasons.Select(s => new SeasonTvSeriesVM
            {
                Id = s.Id,
                Title = s.Title,
                SeasonNumber = s.SeasonNumber,
                EpisodeCount = s.Episodes.Count(e => !e.IsDeleted),
                IsDeleted = s.IsDeleted,
                CurrentState = s.CurrentState.Value
            }).OrderBy(s => s.SeasonNumber).ToList();

            var charactersVM = tvSeries.Characters.Select(c => new CharacterTvSeriesVM
            {
                Id = c.Character.Id,
                Name = c.Character.Name,
                ActorName = c.Character.Name,
                Description = c.Character.Description,
                ImageUrl = c.Character.ImgUrl
            }).ToList();

            return new TvSeriesAdminDetailsVM
            {
                Id = tvSeries.Id,
                Title = tvSeries.Title,
                Description = tvSeries.Description,
                Author = tvSeries.Author,
                ImgUrl = tvSeries.ImgUrl,
                ReleaseYear = tvSeries.ReleaseYear,
                Rating = tvSeries.Rating,
                IsDeleted = tvSeries.IsDeleted,
                CurrentState = tvSeries.CurrentState.Value,
                CreatedDateUtc = tvSeries.CreatedDateUtc,
                CreatedBy = tvSeries.CreatedBy,
                LastModifiedDateUtc = tvSeries.UpdatedDateUtc,
                LastModifiedBy = tvSeries.UpdatedBy,
                DeletedDateUtc = tvSeries.DeletedDateUtc,
                DeletedBy = tvSeries.DeletedBy,
                Seasons = seasonsVM,
                Characters = charactersVM
            };
        }


        public async Task<TvSeries> CreateTvSeriesAsync(TvSeriesAdminCreateVM viewModel)
        {
            var userName = _HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var tvSeries = new TvSeries
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                Author = viewModel.Author,
                ImgUrl = viewModel.ImgUrl,
                ReleaseYear = viewModel.ReleaseYear,
                CurrentState = viewModel.CurrentState,
                CreatedDateUtc = DateTime.UtcNow,
                CreatedBy = userName
            };

            return await _tvSeriesRepository.Add(tvSeries);
        }


        public async Task UpdateTvSeriesAsync(Guid id, TvSeriesAdminEditVM viewModel)
        {
            var userName = _HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var tvSeries = await _tvSeriesRepository.GetByIdAsync(id);
            if (tvSeries == null)
            {
                throw new KeyNotFoundException("TV Series not found");
            }

            tvSeries.Title = viewModel.Title;
            tvSeries.Description = viewModel.Description;
            tvSeries.Author = viewModel.Author;
            tvSeries.ImgUrl = viewModel.ImgUrl;
            tvSeries.ReleaseYear = viewModel.ReleaseYear;
            tvSeries.CurrentState = viewModel.CurrentState;
            tvSeries.UpdatedBy = userName;
            tvSeries.UpdatedDateUtc = DateTime.UtcNow;

            await _tvSeriesRepository.Update(tvSeries);
        }


        public async Task SoftDelete(Guid Id)
        {
            await _tvSeriesRepository.SoftDeleteAsync(Id);
        }


        public async Task Delete(Guid Id)
        {
            await _tvSeriesRepository.DeleteInDB(Id);
        }


        public async Task RestoreAsync(Guid Id)
        {
            await _tvSeriesRepository.RestoreAsync(Id);
        }


    }
}

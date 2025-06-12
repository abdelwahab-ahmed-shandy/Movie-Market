using Castle.Core.Logging;
using DAL.ViewModels.Special;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Customer
{
    public class CustomerSpecialService : ICustomerSpecialService
    {
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly ILogger<CustomerSpecialService> _logger;

        public CustomerSpecialService(IGenericRepository<Special> specialRepo,
                                            ILogger<CustomerSpecialService> logger)
        {
            _logger = logger;
            _specialRepo = specialRepo;
        }


        public async Task<IEnumerable<SpecialIndexVM>> GetActiveSpecialAsync()
        {
            try
            {
                var now = DateTime.Now;

                var specials = await _specialRepo
                    .Get(s => !s.IsDeleted &&
                         s.CurrentState == CurrentState.Active &&
                         s.StartDate <= now &&
                         s.EndDate >= now)
                    .OrderBy(s => s.StartDate)
                    .ToListAsync();

                return specials.Select(s => new SpecialIndexVM
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DiscountPercentage = s.DiscountPercentage,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active specials");
                throw;
            }
        }

        public async Task<SpecialDetailsVM?> GetSpecialDetailsAsync(Guid id)
        {
            try
            {
                var special = await _specialRepo.GetAll()
                    .Include(s => s.MovieSpecials)
                        .ThenInclude(ms => ms.ParentMovie)
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

                if (special == null)
                    return null;

                return new SpecialDetailsVM
                {
                    Id = special.Id,
                    Name = special.Name,
                    Description = special.Description,
                    StartDate = special.StartDate,
                    EndDate = special.EndDate,
                    DiscountPercentage = special.DiscountPercentage,
                    Movies = special.MovieSpecials
                        .Where(ms => !ms.IsDeleted && !ms.ParentMovie.IsDeleted)
                        .OrderBy(ms => ms.DisplayOrder)
                        .Select(ms => new SpecialMovieVM
                        {
                            MovieId = ms.ParentMovieId,
                            MovieTitle = ms.ParentMovie.Title,
                            MoviePosterUrl = ms.ParentMovie.ImgUrl,
                            IsFeatured = ms.IsFeatured,
                            DisplayOrder = ms.DisplayOrder
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting special details for ID: {id}");
                throw;
            }
        }



    }
}

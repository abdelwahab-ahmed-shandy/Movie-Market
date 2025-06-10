using BLL.Services.Interfaces.Customer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Customer
{
    public class CustomerCinemaService : ICustomerCinemaService
    {
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly ILogger<CustomerCinemaService> _logger;

        public CustomerCinemaService(
            IGenericRepository<Cinema> cinemaRepo,
            ILogger<CustomerCinemaService> logger)
        {
            _cinemaRepo = cinemaRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<CinemaIndexVM>> GetActiveCinemaAsync()
        {
            try
            {
                var cinemas = await _cinemaRepo.GetAll()
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return cinemas.Select(c => new CinemaIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Description = c.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active cinemas");
                throw;
            }
        }

        public async Task<CinemaDetailsVM?> GetCinemaDetailsAsync(Guid id)
        {
            try
            {
                var cinema = await _cinemaRepo.GetAll()
                    .Include(c => c.CinemaMovies)
                        .ThenInclude(cm => cm.Movie)
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (cinema == null)
                    return null;

                var cinemaDetails = new CinemaDetailsVM
                {
                    Id = cinema.Id,
                    Name = cinema.Name,
                    Location = cinema.Location,
                    Description = cinema.Description,
                    CreatedDate = cinema.CreatedDateUtc
                };

                foreach (var cinemaMovie in cinema.CinemaMovies.Where(cm => !cm.IsDeleted))
                {
                    cinemaDetails.Movies.Add(new MovieDetailsVM
                    {
                        Id = cinemaMovie.Movie.Id,
                        Title = cinemaMovie.Movie.Title,
                        ImgUrl = cinemaMovie.Movie.ImgUrl,
                        StartDate = cinemaMovie.ShowTime,
                        Duration = cinemaMovie.Movie.Duration
                    });
                }

                return cinemaDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting cinema details for ID: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<CinemaIndexVM>> GetPopularCinemaAsync(int count)
        {
            try
            {
                // This is a simplified version - you might want to implement actual popularity logic
                var cinemas = await _cinemaRepo.GetAll()
                    .OrderByDescending(c => c.CreatedDateUtc)
                    .Take(count)
                    .ToListAsync();

                return cinemas.Select(c => new CinemaIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Description = c.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting top {count} popular cinemas");
                throw;
            }
        }

    }
}

using BLL.Services.Interfaces.Customer;
using Microsoft.EntityFrameworkCore;
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
        private readonly IGenericRepository<Movie> _movieRepo;
        public CustomerCinemaService(IGenericRepository<Cinema> cinemaRepo, IGenericRepository<Movie> movieRepo,
            ILogger<CustomerCinemaService> logger)
        {
            _cinemaRepo = cinemaRepo;
            _logger = logger;
            _movieRepo = movieRepo;
        }


        public async Task<IEnumerable<CinemaIndexVM>> GetActiveCinemaAsync()
        {
            try
            {
                var cinemas = await _cinemaRepo.Get(c => !c.IsDeleted && c.CurrentState.Value == DAL.Enums.CurrentState.Active)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return cinemas.Select(c => new CinemaIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Description = c.Description,
                    ImageUrl = c.ImgUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active cinemas");
                throw;
            }
        }


        public async Task<CinemaDetailsVM?> GetCinemaDetailsAsync(Guid id,string? searchString = null,string? sortOrder = null)
        {
            try
            {
                var cinema = await _cinemaRepo.GetAll()
                    .Include(c => c.CinemaMovies)
                        .ThenInclude(cm => cm.Movie)
                        .ThenInclude(m => m.Category)
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (cinema == null)
                    return null;

                var cinemaDetails = new CinemaDetailsVM
                {
                    Id = cinema.Id,
                    Name = cinema.Name,
                    Location = cinema.Location,
                    Description = cinema.Description,
                    CreatedDate = cinema.CreatedDateUtc,
                    Img = cinema.ImgUrl
                };

                var moviesQuery = _movieRepo.GetAll()
                    .Where(m => m.CinemaMovies.Any(cm => cm.CinemaId == id && !cm.IsDeleted))
                    .Include(m => m.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    moviesQuery = moviesQuery.Where(m =>
                        m.Title.Contains(searchString) ||
                        (m.Description != null && m.Description.Contains(searchString)));
                }

                moviesQuery = sortOrder switch
                {
                    "title" => moviesQuery.OrderBy(m => m.Title),
                    "title_desc" => moviesQuery.OrderByDescending(m => m.Title),
                    "date" => moviesQuery.OrderByDescending(m => m.StartDate),
                    "date_desc" => moviesQuery.OrderBy(m => m.EndDate),
                    "rating" => moviesQuery.OrderByDescending(m => m.Rating),
                    _ => moviesQuery.OrderBy(m => m.Title) 
                };

                cinemaDetails.Movies = await moviesQuery
                    .Select(m => new MovieDetailsVM
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        Price = m.Price,
                        Author = m.Author,
                        ImgUrl = m.ImgUrl,
                        Duration = m.Duration,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        ReleaseYear = m.ReleaseYear,
                        Rating = m.Rating,
                        CategoryName = m.Category.Name
                    })
                    .ToListAsync(); 

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

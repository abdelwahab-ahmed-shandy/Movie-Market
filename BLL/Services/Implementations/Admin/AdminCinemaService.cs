using BLL.Services.Interfaces.Admin;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminCinemaService : IAdminCinemaService
    {
        private readonly IGenericRepository<Cinema> _cinemaRepo;

        public AdminCinemaService(IGenericRepository<Cinema> cinemaRepo)
        {
            _cinemaRepo = cinemaRepo;
        }


        public async Task<IEnumerable<CinemaAdminVM>> GetAllAsync()
        {
            var cinemas = await _cinemaRepo.GetAll().ToListAsync();
            return cinemas.Select(c => new CinemaAdminVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Location = c.Location
            });
        }


        public async Task<CinemaAdminVM?> GetByIdAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(id);
            if (cinema == null) return null;

            return new CinemaAdminVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Description = cinema.Description,
                Location = cinema.Location
            };
        }


        public async Task<CinemaAdminVM> CreateAsync(CinemaAdminVM cinemaVM)
        {
            var cinema = new Cinema
            {
                Name = cinemaVM.Name,
                Description = cinemaVM.Description,
                Location = cinemaVM.Location
            };

            await _cinemaRepo.Add(cinema);

            cinemaVM.Id = cinema.Id;
            return cinemaVM;
        }


        public async Task<CinemaAdminVM> UpdateAsync(CinemaAdminVM cinemaVM)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(cinemaVM.Id);
            if (cinema == null)
                throw new Exception("Cinema not found");

            cinema.Name = cinemaVM.Name;
            cinema.Description = cinemaVM.Description;
            cinema.Location = cinemaVM.Location;

            await _cinemaRepo.Update(cinema);
            return cinemaVM;
        }


        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            await _cinemaRepo.SoftDeleteAsync(id);
            return true;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(id);
            if (cinema == null)
                return false;
            await _cinemaRepo.DeleteInDB(id);
            return true;
        }


    }
}

using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Admin
{
    public interface IAdminCinemaService
    {
        Task<CinemaAdminListVM> GetAllCinemasAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null);
        Task<CinemaAdminDetailsVM?> GetCinemaDetailsAsync(Guid id);
        Task<CinemaAdminVM?> GetByIdAsync(Guid id);
        Task<CinemaAdminVM> CreateAsync(CinemaAdminVM cinema);
        Task<CinemaAdminVM> UpdateAsync(CinemaAdminVM cinema);
        Task SoftDeleteAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}

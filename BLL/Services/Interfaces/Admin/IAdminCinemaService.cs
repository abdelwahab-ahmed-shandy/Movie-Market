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
        Task<CinemaAdminListVM> GetAllCinemasAsync(int page, int pageSize, string query = null);
        Task<CinemaAdminDetailsVM?> GetCinemaDetailsAsync(Guid id);
        Task<CinemaAdminVM?> GetByIdAsync(Guid id);
        Task<CinemaAdminVM> CreateAsync(CinemaAdminVM cinema);
        Task<CinemaAdminVM> UpdateAsync(CinemaAdminVM cinema);
        Task SoftDeleteAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}

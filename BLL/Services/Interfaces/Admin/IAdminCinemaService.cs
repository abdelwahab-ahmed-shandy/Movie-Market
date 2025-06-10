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
        Task<IEnumerable<CinemaAdminVM>> GetAllAsync();
        Task<CinemaAdminVM?> GetByIdAsync(Guid id);
        Task<CinemaAdminVM> CreateAsync(CinemaAdminVM cinema);
        Task<CinemaAdminVM> UpdateAsync(CinemaAdminVM cinema);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}

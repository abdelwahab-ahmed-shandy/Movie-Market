using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Admin
{
    public interface IAdminSpecialService
    {
        Task<SpecialAdminListVM> GetAllSpecialsAsync(int page, int pageSize, string? query = null);
        Task<SpecialAdminDetailsVM?> GetSpecialByIdAsync(Guid id);
        Task<Guid> CreateSpecialAsync(SpecialCreateVM model);
        Task<bool> UpdateSpecialAsync(Guid id, SpecialEditVM model);
        Task<bool> ToggleSpecialStatusAsync(Guid id);
        Task SoftDeleteAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}

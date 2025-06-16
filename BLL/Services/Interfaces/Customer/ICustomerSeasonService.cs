using DAL.ViewModels.Season;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerSeasonService
    {
        Task<List<SeasonCustomerVM>> GetAllSeasonAsync();
        Task<SeasonDetailsVM?> GetSeasonDetailsAsync(Guid id);
    }
}

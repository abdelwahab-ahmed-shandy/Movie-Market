using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerCinemaService
    {
        Task<IEnumerable<CinemaIndexVM>> GetActiveCinemaAsync();
        Task<CinemaDetailsVM?> GetCinemaDetailsAsync(Guid id, string? searchString = null, string? sortOrder = null);
        Task<IEnumerable<CinemaIndexVM>> GetPopularCinemaAsync(int count);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerMovieService
    {
        Task<IEnumerable<MovieIndexVM>> GetActiveMoviesAsync();

        Task<MovieDetailsVM?> GetMovieDetailsAsync(Guid id);

        Task<IEnumerable<MovieIndexVM>> GetPopularMoviesAsync(int count);

        Task<IEnumerable<MovieIndexVM>> GetMoviesByCategoryAsync(Guid categoryId);

    }
}

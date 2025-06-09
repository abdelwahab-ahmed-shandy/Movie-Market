using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAdminMovieService
    {
        Task<IEnumerable<MovieAdminListVM>> GetAllMoviesAsync();
        Task<MovieAdminEditVM?> GetMovieForEditAsync(Guid id);
        Task<bool> CreateMovieAsync(MovieAdminCreateVM model);
        Task<bool> UpdateMovieAsync(MovieAdminEditVM model);
        Task<bool> SoftDeleteMovieAsync(Guid id);
        Task<bool> RestoreMovieAsync(Guid id);
        Task<bool> DeleteMoviePermanentlyAsync(Guid id);
    }
}

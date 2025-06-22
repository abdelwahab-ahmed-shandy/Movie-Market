using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMovieService
    {

        #region Admin Methods
        Task<MovieAdminListResultVM> GetAllMoviesAsync(int page, int pageSize, string? query = null);
        Task<MovieAdminDetailsVM?> GetMovieDetailsAsync(Guid id);
        Task<MovieAdminEditVM?> GetMovieForEditAsync(Guid id);
        Task<bool> CreateMovieAsync(MovieAdminCreateVM model);
        Task<bool> UpdateMovieAsync(MovieAdminEditVM model);
        Task<bool> SoftDeleteMovieAsync(Guid id); 
        Task<bool> RestoreMovieAsync(Guid id);
        Task<bool> DeleteMoviePermanentlyAsync(Guid id);
        #endregion

        #region Customer Methods

        Task<IEnumerable<MovieIndexVM>> GetActiveMoviesAsync();

        Task<List<MovieDetailsVM>> GetMoviesNewReleasesAsync(int count);

        Task<MovieDetailsVM?> GetMovieWithDetailsAsync(Guid id);

        Task<IEnumerable<MovieIndexVM>> GetPopularMoviesAsync(int count);

        Task<IEnumerable<MovieIndexVM>> GetMoviesByCategoryAsync(Guid categoryId);

        #endregion
        
    }
}

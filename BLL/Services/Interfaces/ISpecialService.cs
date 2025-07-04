
namespace BLL.Services.Interfaces
{
    public interface ISpecialService
    {

        #region Admin Methods
        Task<SpecialAdminListVM> GetAllSpecialsAsync(int page, int pageSize, string? query = null);
        Task<SpecialAdminDetailsVM?> GetSpecialByIdAsync(Guid id);
        Task<Guid> CreateSpecialAsync(SpecialCreateVM model);
        Task<bool> UpdateSpecialAsync(Guid id, SpecialEditVM model);
        Task<bool> ToggleSpecialStatusAsync(Guid id);
        Task SoftDeleteAsync(Guid id);
        Task DeleteAsync(Guid id);
        #endregion


        #region Customer Methods
        Task<IEnumerable<SpecialIndexVM>> GetActiveSpecialAsync();
        Task<SpecialDetailsVM?> GetSpecialDetailsAsync(Guid id);
        #endregion
   
    }
}
 
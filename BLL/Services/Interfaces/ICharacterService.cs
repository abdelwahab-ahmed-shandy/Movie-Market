using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICharacterService
    {

        #region Admin Methods
        Task<CharacterAdminListVM> GetCharacterAllAsync(int page, int pageSize, string? query = null);
        Task<CharacterAdminVM> GetCharacterByIdAsync(Guid id);
        Task<bool> CreateCharacterAsync(CreateCharacterVM model);
        Task<bool> UpdateCharacterAsync(EditCharacterVM model);
        Task<bool> SoftDeleteCharacterAsync(Guid id);
        Task<bool> RestoreCharacterAsync(Guid id); 
        Task<bool> DeleteCharacterAsync(Guid id);
        #endregion


        #region Customer Methods

        Task<List<CharacterIndexVM>> GetAllCharactersAsync();
        Task<CharacterIndexVM?> GetCharacterDetailsAsync(Guid id);
        #endregion
    
    }
}

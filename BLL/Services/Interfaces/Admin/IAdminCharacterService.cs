using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Admin
{
    public interface IAdminCharacterService
    {
        Task<CharacterAdminListVM> GetCharacterAll(int page, int pageSize, string? query = null);
        Task<CharacterAdminVM> GetCharacterById(Guid id);
        Task<bool> CreateCharacter(CreateCharacterVM model);
        Task<bool> UpdateCharacter(EditCharacterVM model);
        Task<bool> SoftDeleteCharacter(Guid id);
        Task<bool> RestoreCharacter(Guid id);
        Task<bool> DeleteCharacter(Guid id);
    }
}

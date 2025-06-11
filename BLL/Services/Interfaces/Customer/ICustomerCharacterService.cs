using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerCharacterService
    {
        Task<List<CharacterIndexVM>> GetAllCharactersAsync();
        Task<CharacterIndexVM?> GetCharacterDetailsAsync(Guid id);
    }
}

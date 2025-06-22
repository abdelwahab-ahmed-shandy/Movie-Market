using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICharacterTvSeriesService
    {
        Task<CharacterTvSeriesAddVM> GetAddCharactersViewModelAsync(Guid tvSeriesId);
        Task AddCharacterToTvSeriesAsync(Guid tvSeriesId, Guid characterId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Admin
{
    public interface IAdminCharacterTvSeriesService
    {
        Task<CharacterTvSeriesAddVM> GetAddCharactersViewModel(Guid tvSeriesId);
        Task AddCharacterToTvSeries(Guid tvSeriesId, Guid characterId);
    }
}

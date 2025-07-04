
namespace BLL.Services.Interfaces
{
    public interface ICharacterTvSeriesService
    {
        Task<CharacterTvSeriesAddVM> GetAddCharactersViewModelAsync(Guid tvSeriesId);
        Task AddCharacterToTvSeriesAsync(Guid tvSeriesId, Guid characterId);
    }
}

﻿
namespace BLL.Services.Implementations
{
    public class CharacterTvSeriesService : ICharacterTvSeriesService
    {
        private readonly IGenericRepository<Character> _characterRepository;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepository;
        private readonly IGenericRepository<CharacterTvSeries> _characterTvSeriesRepository;

        public CharacterTvSeriesService(
            IGenericRepository<Character> characterRepository,
            IGenericRepository<TvSeries> tvSeriesRepository,
            IGenericRepository<CharacterTvSeries> characterTvSeriesRepository)
        {
            _characterRepository = characterRepository;
            _tvSeriesRepository = tvSeriesRepository;
            _characterTvSeriesRepository = characterTvSeriesRepository;
        }

        public async Task<CharacterTvSeriesAddVM> GetAddCharactersViewModelAsync(Guid tvSeriesId)
        {
            var tvSeries = await _tvSeriesRepository.GetByIdAsync(tvSeriesId);
            if (tvSeries == null)
            {
                throw new KeyNotFoundException("TV Series not found");
            }

            var existingCharacterIds = tvSeries.Characters.Select(c => c.CharacterId).ToList();

            var availableCharacters = await _characterRepository.GetAll()
                .Where(c => !existingCharacterIds.Contains(c.Id))
                .Select(c => new CharacterSelectionVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    ActorName = c.Name
                })
                .ToListAsync();

            return new CharacterTvSeriesAddVM
            {
                TvSeriesId = tvSeriesId,
                TvSeriesTitle = tvSeries.Title,
                AvailableCharacters = availableCharacters
            };
        }

        public async Task AddCharacterToTvSeriesAsync(Guid tvSeriesId, Guid characterId)
        {
            var exists = await _characterTvSeriesRepository.GetAll()
                .AnyAsync(cts => cts.TvSeriesId == tvSeriesId && cts.CharacterId == characterId);

            if (exists)
            {
                throw new InvalidOperationException("This character is already associated with the TV series");
            }

            var characterTvSeries = new CharacterTvSeries
            {
                TvSeriesId = tvSeriesId,
                CharacterId = characterId
            };

            await _characterTvSeriesRepository.Add(characterTvSeries);
        }
   
    }
}

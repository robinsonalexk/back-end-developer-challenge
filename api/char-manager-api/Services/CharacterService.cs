using CharacterManagerAPI.Hubs;
using CharManagerAPI.Models;
using CharManagerAPI.Models.Enums;
using CharManagerAPI.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace CharManagerAPI.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IHubContext<CharacterHub, ICharacterClient> _hubContext;

        public CharacterService(ICharacterRepository characterRepository, IHubContext<CharacterHub, ICharacterClient> hubContext)
        {
            _characterRepository = characterRepository;
            _hubContext = hubContext;
        }

        public Task<List<Character>> GetAllAsync() => _characterRepository.GetAllAsync();

        public Task<Character?> GetByIdAsync(string id) => _characterRepository.GetByIdAsync(id);

        public async Task<Character> CreateAsync(Character character)
        {
            throw new NotImplementedException();
        }

        public async Task<Character> UpdateAsync(Character character)
        {
            throw new NotImplementedException();
        }


        public async Task<string> DeleteAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DealDamageAsync(string characterName, int amount, DamageType type)
        {
            var character = await _characterRepository.GetByIdAsync(characterName);
            if (character != null)
            {
                var defenses = character.Defenses.FindAll(d => d.Type == type);
                if (defenses.Any(d => d.Defense == DefenseType.Immunity))
                {
                    amount = 0;
                }
                if (defenses.Any(d => d.Defense == DefenseType.Resistance))
                {
                    amount /= 2;
                }

                var tempHp = character.TempHitPoints;
                var tempDelta = tempHp - amount;

                var currentHp = tempDelta < 0 ? character.CurrentHitPoints + tempDelta : character.CurrentHitPoints;

                character.TempHitPoints = tempDelta < 0 ? 0 : tempDelta;
                character.CurrentHitPoints = currentHp < 0 ? 0 : currentHp;

                var result = await _characterRepository.UpdateAsync(character);
                if (result)
                    await _hubContext.Clients.All.CharacterUpserted(character);
                return result;
            }

            throw new KeyNotFoundException();
        }

        public async Task<bool> HealDamageAsync(string characterName, int amount)
        {
            var character = await _characterRepository.GetByIdAsync(characterName);
            if (character != null)
            {
                var maxHp = character.HitPoints;
                var currentHp = character.CurrentHitPoints + amount;
                if (currentHp > maxHp)
                    currentHp -= currentHp - maxHp;

                character.CurrentHitPoints = currentHp;

                var result = await _characterRepository.UpdateAsync(character);
                if (result)
                    await _hubContext.Clients.All.CharacterUpserted(character);
                return result;
            }

            throw new KeyNotFoundException();
        }

        public async Task<bool> AddTempHpAsync(string characterName, int amount)
        {
            var character = await _characterRepository.GetByIdAsync(characterName);
            if (character != null)
            {
                var tempHp = character.TempHitPoints;
                if (amount > tempHp)
                    tempHp = amount;

                character.TempHitPoints = tempHp;

                var result = await _characterRepository.UpdateAsync(character);
                if (result)
                    await _hubContext.Clients.All.CharacterUpserted(character);
                return result;
            }

            throw new KeyNotFoundException();
        }
    }
}
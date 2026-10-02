using CharManagerAPI.Models;
using CharManagerAPI.Models.Enums;

namespace CharManagerAPI.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(string name);
        Task<Character> CreateAsync(Character character);
        Task<Character> UpdateAsync(Character character);
        Task<string> DeleteAsync(string name);
        Task<bool> DealDamageAsync(string characterName, int amount, DamageType type);
        Task<bool> HealDamageAsync(string characterName, int amount);
        Task<bool> AddTempHpAsync(string characterName, int amount);
    }
}
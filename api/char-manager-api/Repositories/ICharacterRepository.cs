using CharManagerAPI.Models;

namespace CharManagerAPI.Repositories
{
    public interface ICharacterRepository
    {
        Task<List<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(string name);
        Task CreateAsync(Character character);
        Task<bool> UpdateAsync(Character character);
        Task<bool> DeleteAsync(string id);
    }
}
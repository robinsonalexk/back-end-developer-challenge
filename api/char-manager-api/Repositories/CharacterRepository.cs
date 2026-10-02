using CharManagerAPI.Models;
using MongoDB.Driver;

namespace CharManagerAPI.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IMongoCollection<Character> _character;

        public CharacterRepository(IMongoDatabase db)
        {
            _character = db.GetCollection<Character>("characters");
        }

        public Task<List<Character>> GetAllAsync() =>
            _character.Find(_ => true).ToListAsync();

        public async Task<Character?> GetByIdAsync(string name) =>
            await _character.Find(c => c.Name == name).FirstOrDefaultAsync();

        public Task CreateAsync(Character character) =>
            _character.InsertOneAsync(character);

        public async Task<bool> UpdateAsync(Character character)
        {

            var result = await _character.ReplaceOneAsync(c => c.Name == character.Name, character);
            return result.MatchedCount > 0;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            var result = await _character.DeleteOneAsync(c => c.Name == name);
            return result.DeletedCount > 0;
        }
    }
}
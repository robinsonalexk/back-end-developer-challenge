using MongoDB.Bson.Serialization.Attributes;

namespace CharManagerAPI.Models
{
    public class Character
    {
        [BsonId]
        public required string Name { get; set; }
        public int Level { get; set; }
        public int HitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
        public int TempHitPoints { get; set; }
        public Stats Stats { get; set; } = new();
        public List<Class> Classes { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public List<Defenses> Defenses { get; set; } = new();
    }
}
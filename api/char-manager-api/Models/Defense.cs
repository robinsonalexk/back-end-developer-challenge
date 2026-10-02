using CharManagerAPI.Models.Enums;

namespace CharManagerAPI.Models
{
    public class Defenses
    {
        public required DamageType Type { get; set; }
        public DefenseType Defense { get; set; }
    }

    public enum DefenseType
    {
        Immunity,
        Resistance
    }
}
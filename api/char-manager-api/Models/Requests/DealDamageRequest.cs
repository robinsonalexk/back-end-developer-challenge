using System.ComponentModel.DataAnnotations;
using CharManagerAPI.Models.Enums;

namespace CharManagerAPI.Models.Requests
{
    public record DealDamageRequest
    {
        [Required]
        public DamageType? IncomingDamageType { get; init; }

        [Range(1, int.MaxValue)]
        public int Amount { get; init; }
    };
}
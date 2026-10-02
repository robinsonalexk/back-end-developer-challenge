using System.ComponentModel.DataAnnotations;

namespace CharManagerAPI.Models.Requests
{
    public record HealDamageRequest
    {
        [Range(1, int.MaxValue)]
        public int Amount { get; init; }
    };
}
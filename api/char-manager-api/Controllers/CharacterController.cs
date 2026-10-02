using CharManagerAPI.Models;
using CharManagerAPI.Models.Enums;
using CharManagerAPI.Models.Requests;
using CharManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CharManagerAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Character>>> GetAll() =>
            Ok(await _characterService.GetAllAsync());

        [HttpPut("{name}/damage")]
        public async Task<IActionResult> DealDamage(string name, DealDamageRequest request)
        {
            if (request.IncomingDamageType is not DamageType damageType)
            {
                return BadRequest("IncomingDamageType is required.");
            }

            var result = await _characterService.DealDamageAsync(name, request.Amount, damageType);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{name}/heal")]
        public async Task<IActionResult> HealDamage(string name, HealDamageRequest request)
        {
            var result = await _characterService.HealDamageAsync(name, request.Amount);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{name}/temp-heal")]
        public async Task<IActionResult> AddTempHp(string name, HealDamageRequest request)
        {
            var result = await _characterService.AddTempHpAsync(name, request.Amount);
            return result ? NoContent() : NotFound();
        }
    }
}
using Microsoft.AspNetCore.SignalR;

namespace CharacterManagerAPI.Hubs
{
    public class CharacterHub : Hub<ICharacterClient>
    {
    }
}
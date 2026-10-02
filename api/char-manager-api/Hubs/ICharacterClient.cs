using CharManagerAPI.Models;

public interface ICharacterClient
{
    Task CharacterUpserted(Character character);
    Task CharacterRemoved(string name);
}
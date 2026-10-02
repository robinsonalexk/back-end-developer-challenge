namespace CharManagerAPI.Models
{
    public class Item
    {
        public required string Name { get; set; }
        public Modifier Modifier { get; set; } = new();
    }
}

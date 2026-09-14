namespace PokemonTournamentApi.Model
{
    public enum Type
    {
        Water = 0,
        Fire,
        Grass,
        Electric,
        Psychic,
        Fighting,
        Dark,
        Ghost
    }

    public class PokemonType2
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}

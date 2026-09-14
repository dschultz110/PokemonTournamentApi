using System.Text.Json.Serialization;

namespace PokemonTournament.Core.Services
{
    public class PokeApiPokemonResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("base_experience")]
        public int BaseExperience { get; set; }

        public List<PokeApiTypeSlot> Types { get; set; } = new();
    }

    public class PokeApiTypeSlot
    {
        public int Slot { get; set; }

        public PokeApiTypeInfo Type { get; set; } = new();
    }

    public class PokeApiTypeInfo
    {
        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}

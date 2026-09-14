using System.Text.Json.Serialization;

namespace PokemonTournamentApi.Model
{
    public class Pokemon
    {
        int Id { get; set; }
        string Name { get; set; }

        [JsonPropertyName("base_experience")]
        int BaseExperience { get; set; }
        IEnumerable<PokemonType> Types { get; set; }
    }
}

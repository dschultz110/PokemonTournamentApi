using PokemonTournament.Core.Services;

namespace PokemonTournament.Core.Models
{
    public class Pokemon(int id, string name, ElementType type, int baseExperience)
    {
        public int Id { get; } = id;

        public string Name { get; } = name;

        public ElementType Type { get; } = type;

        public int BaseExperience { get; } = baseExperience;

        public static Pokemon FromApiResponse(PokeApiPokemonResponse response)
        {
            var primaryType = response.Types.Single(t => t.Slot == 1);

            if (!Enum.TryParse<ElementType>(primaryType.Type.Name, ignoreCase: true, out var elementType))
            {
                throw new NotSupportedException(
                    $"Pokémon '{response.Name}' has unsupported primary type '{primaryType.Type.Name}'.");
            }

            return new Pokemon(response.Id, response.Name, elementType, response.BaseExperience);
        }
    }
}

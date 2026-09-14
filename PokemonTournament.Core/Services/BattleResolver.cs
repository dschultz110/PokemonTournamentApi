using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public class BattleResolver : IBattleResolver
    {
        private static readonly Dictionary<ElementType, ElementType> WinningTypeMap = new()
        {
            [ElementType.Water] = ElementType.Fire,
            [ElementType.Fire] = ElementType.Grass,
            [ElementType.Grass] = ElementType.Electric,
            [ElementType.Electric] = ElementType.Water,
            [ElementType.Ghost] = ElementType.Psychic,
            [ElementType.Psychic] = ElementType.Fighting,
            [ElementType.Fighting] = ElementType.Dark,
            [ElementType.Dark] = ElementType.Ghost,
        };

        public BattleOutcome Resolve(Pokemon pokemonA, Pokemon pokemonB)
        {
            if (WinningTypeMap.TryGetValue(pokemonA.Type, out var typeBeatenByA) && typeBeatenByA == pokemonB.Type)
            {
                return BattleOutcome.PokemonAWins;
            }

            if (WinningTypeMap.TryGetValue(pokemonB.Type, out var typeBeatenByB) && typeBeatenByB == pokemonA.Type)
            {
                return BattleOutcome.PokemonBWins;
            }

            return ResolveByBaseExperience(pokemonA, pokemonB);
        }

        private static BattleOutcome ResolveByBaseExperience(Pokemon pokemonA, Pokemon pokemonB)
        {
            if (pokemonA.BaseExperience > pokemonB.BaseExperience)
            {
                return BattleOutcome.PokemonAWins;
            }

            if (pokemonB.BaseExperience > pokemonA.BaseExperience)
            {
                return BattleOutcome.PokemonBWins;
            }

            return BattleOutcome.Tie;
        }
    }
}

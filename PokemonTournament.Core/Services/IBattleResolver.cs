using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public enum BattleOutcome
    {
        PokemonAWins,
        PokemonBWins,
        Tie
    }

    public interface IBattleResolver
    {
        BattleOutcome Resolve(Pokemon pokemonA, Pokemon pokemonB);
    }
}

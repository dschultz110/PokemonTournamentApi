using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public interface IPokeApiClient
    {
        Task<Pokemon> GetPokemonAsync(int id, CancellationToken cancellationToken = default);
    }
}

using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public interface ITournamentService
    {
        Task<IReadOnlyList<PokemonStatistic>> RunTournamentAsync(CancellationToken cancellationToken = default);
    }
}

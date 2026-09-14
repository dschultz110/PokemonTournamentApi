using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public class TournamentService(IPokeApiClient pokeApiClient, IBattleResolver battleResolver) : ITournamentService
    {
        private const int ParticipantCount = 16;
        private const int MinPokemonId = 1;
        private const int MaxPokemonId = 151;

        private readonly IPokeApiClient _pokeApiClient = pokeApiClient;
        private readonly IBattleResolver _battleResolver = battleResolver;

        public async Task<IReadOnlyList<PokemonStatistic>> RunTournamentAsync(CancellationToken cancellationToken = default)
        {
            var pokemonIds = GetRandomUniqueIds(ParticipantCount, MinPokemonId, MaxPokemonId);

            var competitors = await Task.WhenAll(
                pokemonIds.Select(id => _pokeApiClient.GetPokemonAsync(id, cancellationToken)));

            var statisticsById = competitors.ToDictionary(p => p.Id, p => new PokemonStatistic(p));

            foreach (var (pokemonA, pokemonB) in GetRoundRobinPairs(competitors))
            {
                switch (_battleResolver.Resolve(pokemonA, pokemonB))
                {
                    case BattleOutcome.PokemonAWins:
                        statisticsById[pokemonA.Id].RecordWin();
                        statisticsById[pokemonB.Id].RecordLoss();
                        break;
                    case BattleOutcome.PokemonBWins:
                        statisticsById[pokemonB.Id].RecordWin();
                        statisticsById[pokemonA.Id].RecordLoss();
                        break;
                    default:
                        statisticsById[pokemonA.Id].RecordTie();
                        statisticsById[pokemonB.Id].RecordTie();
                        break;
                }
            }

            return statisticsById.Values.ToList();
        }

        private static List<int> GetRandomUniqueIds(int count, int min, int max)
        {
            var ids = Enumerable.Range(min, max - min + 1).ToArray();
            Random.Shared.Shuffle(ids);
            return ids.Take(count).ToList();
        }

        /// <summary>
        /// Circle method: fix the first pokemon, rotate the rest by one position each round.
        /// For n competitors this means n-1 rounds of n/2 matches each, every pair exactly once.
        /// </summary>
        private static IEnumerable<(Pokemon, Pokemon)> GetRoundRobinPairs(Pokemon[] competitors)
        {
            var n = competitors.Length;
            var fixedCompetitor = competitors[0];
            var rotation = competitors.Skip(1).ToArray();

            for (var round = 0; round < n - 1; round++)
            {
                yield return (fixedCompetitor, rotation[0]);

                for (var i = 1; i < n / 2; i++)
                {
                    yield return (rotation[i], rotation[rotation.Length - i]);
                }

                var temp = rotation[0];
                Array.Copy(rotation, 1, rotation, 0, rotation.Length - 1);
                rotation[rotation.Length - 1] = temp;
            }
        }
    }
}

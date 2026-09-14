using Moq;
using PokemonTournament.Core.Models;
using PokemonTournament.Core.Services;

namespace PokemonTournament.Core.Tests.Services
{
    public class TournamentServiceTests
    {
        private const int ExpectedParticipantCount = 16;
        private const int ExpectedMatchesPerParticipant = ExpectedParticipantCount - 1;
        private const int ExpectedTotalMatches = ExpectedParticipantCount * ExpectedMatchesPerParticipant / 2;

        private readonly Mock<IPokeApiClient> _pokeApiClient = new();

        private TournamentService CreateTournamentService(IBattleResolver battleResolver)
        {
            _pokeApiClient
                .Setup(c => c.GetPokemonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken _) => new Pokemon(id, $"pokemon-{id}", ElementType.Normal, id * 10));

            return new TournamentService(_pokeApiClient.Object, battleResolver);
        }

        [Fact]
        public async Task RunTournamentAsync_FetchesSixteenUniqueParticipants_WithinValidIdRange()
        {
            var service = CreateTournamentService(new AlwaysTieBattleResolver());

            await service.RunTournamentAsync();

            var requestedIds = _pokeApiClient.Invocations
                .Select(i => (int)i.Arguments[0])
                .ToList();

            Assert.Equal(ExpectedParticipantCount, requestedIds.Count);
            Assert.Equal(requestedIds.Count, requestedIds.Distinct().Count());
            Assert.All(requestedIds, id => Assert.InRange(id, 1, 151));
        }

        [Fact]
        public async Task RunTournamentAsync_ReturnsOneStatisticPerFetchedPokemon()
        {
            var service = CreateTournamentService(new AlwaysTieBattleResolver());

            var statistics = await service.RunTournamentAsync();

            Assert.Equal(ExpectedParticipantCount, statistics.Count);
            Assert.Equal(
                statistics.Select(s => s.Pokemon.Id).OrderBy(id => id),
                statistics.Select(s => s.Pokemon.Id).Distinct().OrderBy(id => id));
        }


        [Fact]
        public async Task RunTournamentAsync_EveryParticipantPlaysEveryOtherExactlyOnce()
        {
            var service = CreateTournamentService(new AlwaysTieBattleResolver());

            var statistics = await service.RunTournamentAsync();

            Assert.All(statistics, s =>
            {
                Assert.Equal(ExpectedMatchesPerParticipant, s.Ties);
                Assert.Equal(0, s.Wins);
                Assert.Equal(0, s.Losses);
            });
        }

        [Fact]
        public async Task RunTournamentAsync_CallsBattleResolverExactlyOncePerUniquePair_NeverPairingAPokemonWithItself()
        {
            var battleResolver = new Mock<IBattleResolver>();
            battleResolver
                .Setup(r => r.Resolve(It.IsAny<Pokemon>(), It.IsAny<Pokemon>()))
                .Returns(BattleOutcome.Tie);

            var service = CreateTournamentService(battleResolver.Object);

            await service.RunTournamentAsync();

            var pairs = battleResolver.Invocations
                .Select(i => ((Pokemon)i.Arguments[0], (Pokemon)i.Arguments[1]))
                .ToList();

            Assert.Equal(ExpectedTotalMatches, pairs.Count);
            Assert.All(pairs, p => Assert.NotEqual(p.Item1.Id, p.Item2.Id));

            var unorderedPairKeys = pairs
                .Select(p => p.Item1.Id < p.Item2.Id ? (p.Item1.Id, p.Item2.Id) : (p.Item2.Id, p.Item1.Id))
                .ToList();
            Assert.Equal(unorderedPairKeys.Count, unorderedPairKeys.Distinct().Count());
        }

        [Fact]
        public async Task RunTournamentAsync_AggregatesWinsAndLosses_ConsistentlyRegardlessOfMatchSlot()
        {
            var service = CreateTournamentService(new LowerIdWinsBattleResolver());

            var statistics = await service.RunTournamentAsync();

            Assert.All(statistics, s => Assert.Equal(ExpectedMatchesPerParticipant, s.Wins + s.Losses + s.Ties));
            Assert.All(statistics, s => Assert.Equal(0, s.Ties));
            Assert.Equal(ExpectedTotalMatches, statistics.Sum(s => s.Wins));

            var bestPokemonId = statistics.Min(s => s.Pokemon.Id);
            var worstPokemonId = statistics.Max(s => s.Pokemon.Id);

            var bestStatistic = statistics.Single(s => s.Pokemon.Id == bestPokemonId);
            var worstStatistic = statistics.Single(s => s.Pokemon.Id == worstPokemonId);

            Assert.Equal(ExpectedMatchesPerParticipant, bestStatistic.Wins);
            Assert.Equal(0, bestStatistic.Losses);
            Assert.Equal(0, worstStatistic.Wins);
            Assert.Equal(ExpectedMatchesPerParticipant, worstStatistic.Losses);
        }

        [Fact]
        public async Task RunTournamentAsync_PropagatesExceptions_WhenPokeApiClientFails()
        {
            _pokeApiClient
                .Setup(c => c.GetPokemonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("PokéAPI unavailable"));

            var service = new TournamentService(_pokeApiClient.Object, new AlwaysTieBattleResolver());

            await Assert.ThrowsAsync<HttpRequestException>(() => service.RunTournamentAsync());
        }

        private sealed class AlwaysTieBattleResolver : IBattleResolver
        {
            public BattleOutcome Resolve(Pokemon pokemonA, Pokemon pokemonB) => BattleOutcome.Tie;
        }

        private sealed class LowerIdWinsBattleResolver : IBattleResolver
        {
            public BattleOutcome Resolve(Pokemon pokemonA, Pokemon pokemonB) =>
                pokemonA.Id < pokemonB.Id ? BattleOutcome.PokemonAWins : BattleOutcome.PokemonBWins;
        }
    }
}

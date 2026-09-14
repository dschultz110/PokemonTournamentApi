using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Tests.Models
{
    public class PokemonStatisticTests
    {
        private static Pokemon CreatePokemon() => new(1, "pikachu", ElementType.Electric, baseExperience: 112);

        [Fact]
        public void NewStatistic_StartsAtZeroForAllCounters()
        {
            var statistic = new PokemonStatistic(CreatePokemon());

            Assert.Equal(0, statistic.Wins);
            Assert.Equal(0, statistic.Losses);
            Assert.Equal(0, statistic.Ties);
        }

        [Fact]
        public void RecordWin_IncrementsWinsOnly()
        {
            var statistic = new PokemonStatistic(CreatePokemon());

            statistic.RecordWin();

            Assert.Equal(1, statistic.Wins);
            Assert.Equal(0, statistic.Losses);
            Assert.Equal(0, statistic.Ties);
        }

        [Fact]
        public void RecordLoss_IncrementsLossesOnly()
        {
            var statistic = new PokemonStatistic(CreatePokemon());

            statistic.RecordLoss();

            Assert.Equal(0, statistic.Wins);
            Assert.Equal(1, statistic.Losses);
            Assert.Equal(0, statistic.Ties);
        }

        [Fact]
        public void RecordTie_IncrementsTiesOnly()
        {
            var statistic = new PokemonStatistic(CreatePokemon());

            statistic.RecordTie();

            Assert.Equal(0, statistic.Wins);
            Assert.Equal(0, statistic.Losses);
            Assert.Equal(1, statistic.Ties);
        }

        [Fact]
        public void RecordOutcomes_AccumulateIndependently_AcrossMultipleCalls()
        {
            var statistic = new PokemonStatistic(CreatePokemon());

            statistic.RecordWin();
            statistic.RecordWin();
            statistic.RecordLoss();
            statistic.RecordTie();
            statistic.RecordTie();
            statistic.RecordTie();

            Assert.Equal(2, statistic.Wins);
            Assert.Equal(1, statistic.Losses);
            Assert.Equal(3, statistic.Ties);
        }
    }
}

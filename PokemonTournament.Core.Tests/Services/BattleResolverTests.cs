using PokemonTournament.Core.Models;
using PokemonTournament.Core.Services;

namespace PokemonTournament.Core.Tests.Services
{
    public class BattleResolverTests
    {
        private readonly BattleResolver _resolver = new();

        public static IEnumerable<object[]> TypeAdvantageCycles => new List<object[]>
        {
            new object[] { ElementType.Water, ElementType.Fire },
            new object[] { ElementType.Fire, ElementType.Grass },
            new object[] { ElementType.Grass, ElementType.Electric },
            new object[] { ElementType.Electric, ElementType.Water },
            new object[] { ElementType.Ghost, ElementType.Psychic },
            new object[] { ElementType.Psychic, ElementType.Fighting },
            new object[] { ElementType.Fighting, ElementType.Dark },
            new object[] { ElementType.Dark, ElementType.Ghost },
        };

        [Theory]
        [MemberData(nameof(TypeAdvantageCycles))]
        public void Resolve_TypeAdvantageWins_EvenWhenDefenderHasHigherBaseExperience(
            ElementType winningType, ElementType losingType)
        {
            var winner = new Pokemon(1, "winner", winningType, baseExperience: 50);
            var loser = new Pokemon(2, "loser", losingType, baseExperience: 500);

            Assert.Equal(BattleOutcome.PokemonAWins, _resolver.Resolve(winner, loser));
        }

        [Theory]
        [MemberData(nameof(TypeAdvantageCycles))]
        public void Resolve_TypeAdvantageIsOrderIndependent_WinnerAlwaysWinsRegardlessOfArgumentPosition(
            ElementType winningType, ElementType losingType)
        {
            var winner = new Pokemon(1, "winner", winningType, baseExperience: 50);
            var loser = new Pokemon(2, "loser", losingType, baseExperience: 500);

            Assert.Equal(BattleOutcome.PokemonBWins, _resolver.Resolve(loser, winner));
        }

        [Fact]
        public void Resolve_UnrelatedTypes_FallsBackToHigherBaseExperience()
        {
            var higherExperience = new Pokemon(1, "higher", ElementType.Normal, baseExperience: 100);
            var lowerExperience = new Pokemon(2, "lower", ElementType.Rock, baseExperience: 50);

            Assert.Equal(BattleOutcome.PokemonAWins, _resolver.Resolve(higherExperience, lowerExperience));
            Assert.Equal(BattleOutcome.PokemonBWins, _resolver.Resolve(lowerExperience, higherExperience));
        }

        [Fact]
        public void Resolve_TypesFromDifferentCycles_AreUnrelated_AndFallBackToBaseExperience()
        {
            // Fire (fire-cycle) vs Psychic (ghost-cycle): neither chart covers the other.
            var fire = new Pokemon(1, "fire", ElementType.Fire, baseExperience: 200);
            var psychic = new Pokemon(2, "psychic", ElementType.Psychic, baseExperience: 100);

            Assert.Equal(BattleOutcome.PokemonAWins, _resolver.Resolve(fire, psychic));
        }

        [Fact]
        public void Resolve_SameType_FallsBackToBaseExperience()
        {
            var higherExperience = new Pokemon(1, "a", ElementType.Water, baseExperience: 80);
            var lowerExperience = new Pokemon(2, "b", ElementType.Water, baseExperience: 40);

            Assert.Equal(BattleOutcome.PokemonAWins, _resolver.Resolve(higherExperience, lowerExperience));
        }

        [Fact]
        public void Resolve_UnrelatedTypesWithEqualBaseExperience_IsATie()
        {
            var a = new Pokemon(1, "a", ElementType.Normal, baseExperience: 100);
            var b = new Pokemon(2, "b", ElementType.Rock, baseExperience: 100);

            Assert.Equal(BattleOutcome.Tie, _resolver.Resolve(a, b));
        }

        [Fact]
        public void Resolve_SameTypeWithEqualBaseExperience_IsATie()
        {
            var a = new Pokemon(1, "a", ElementType.Water, baseExperience: 100);
            var b = new Pokemon(2, "b", ElementType.Water, baseExperience: 100);

            Assert.Equal(BattleOutcome.Tie, _resolver.Resolve(a, b));
        }
    }
}

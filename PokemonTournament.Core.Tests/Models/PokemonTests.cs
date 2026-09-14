using PokemonTournament.Core.Models;
using PokemonTournament.Core.Services;

namespace PokemonTournament.Core.Tests.Models
{
    public class PokemonTests
    {
        [Fact]
        public void FromApiResponse_MapsFieldsAndUsesPrimaryType_WhenPokemonHasMultipleTypes()
        {
            var response = new PokeApiPokemonResponse
            {
                Id = 6,
                Name = "charizard",
                BaseExperience = 267,
                Types = new List<PokeApiTypeSlot>
                {
                    new() { Slot = 2, Type = new PokeApiTypeInfo { Name = "flying" } },
                    new() { Slot = 1, Type = new PokeApiTypeInfo { Name = "fire" } },
                }
            };

            var pokemon = Pokemon.FromApiResponse(response);

            Assert.Equal(6, pokemon.Id);
            Assert.Equal("charizard", pokemon.Name);
            Assert.Equal(267, pokemon.BaseExperience);
            Assert.Equal(ElementType.Fire, pokemon.Type);
        }

        [Fact]
        public void FromApiResponse_ParsesType_CaseInsensitive()
        {
            var response = new PokeApiPokemonResponse
            {
                Id = 7,
                Name = "squirtle",
                BaseExperience = 63,
                Types = new List<PokeApiTypeSlot>
                {
                    new() { Slot = 1, Type = new PokeApiTypeInfo { Name = "WATER" } },
                }
            };

            var pokemon = Pokemon.FromApiResponse(response);

            Assert.Equal(ElementType.Water, pokemon.Type);
        }

        [Fact]
        public void FromApiResponse_Throws_WhenPrimaryTypeIsNotAKnownElementType()
        {
            var response = new PokeApiPokemonResponse
            {
                Id = 999,
                Name = "mystery-pokemon",
                BaseExperience = 1,
                Types = new List<PokeApiTypeSlot>
                {
                    new() { Slot = 1, Type = new PokeApiTypeInfo { Name = "shadow" } },
                }
            };

            Assert.Throws<NotSupportedException>(() => Pokemon.FromApiResponse(response));
        }

        [Fact]
        public void FromApiResponse_Throws_WhenNoSlotOneTypeIsPresent()
        {
            var response = new PokeApiPokemonResponse
            {
                Id = 1,
                Name = "bulbasaur",
                BaseExperience = 64,
                Types = new List<PokeApiTypeSlot>
                {
                    new() { Slot = 2, Type = new PokeApiTypeInfo { Name = "poison" } },
                }
            };

            Assert.Throws<InvalidOperationException>(() => Pokemon.FromApiResponse(response));
        }
    }
}

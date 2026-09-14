using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonTournament.Core.Models;
using PokemonTournament.Core.Services;
using PokemonTournamentApi.Controllers;
using PokemonTournamentApi.Dtos;

namespace PokemonTournamentApi.Tests.Controllers
{
    public class PokemonTournamentControllerTests
    {
        private readonly Mock<ITournamentService> _tournamentService = new();

        private PokemonTournamentController CreateService()
        {
            _tournamentService
                .Setup(s => s.RunTournamentAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildStatistics());

            return new PokemonTournamentController(_tournamentService.Object);
        }

        private static IReadOnlyList<PokemonStatistic> BuildStatistics()
        {
            var entries = new (int Id, string Name, ElementType Type, int Wins, int Losses, int Ties)[]
            {
                (1, "bulbasaur", ElementType.Grass, 6, 6, 3),
                (25, "pikachu", ElementType.Electric, 9, 4, 2),
                (7, "squirtle", ElementType.Water, 2, 10, 3),
            };

            return entries.Select(e =>
            {
                var statistic = new PokemonStatistic(new Pokemon(e.Id, e.Name, e.Type, baseExperience: e.Id * 10));
                for (var i = 0; i < e.Wins; i++) statistic.RecordWin();
                for (var i = 0; i < e.Losses; i++) statistic.RecordLoss();
                for (var i = 0; i < e.Ties; i++) statistic.RecordTie();
                return statistic;
            }).ToList();
        }

        private static string? GetErrorMessage(IActionResult result)
        {
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            return badRequest.Value?.GetType().GetProperty("error")?.GetValue(badRequest.Value) as string;
        }

        [Fact]
        public async Task GetStatistics_ReturnsBadRequest_WhenSortByIsOmitted()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: null, sortDirection: "asc");

            Assert.Equal("sortBy parameter is required", GetErrorMessage(result));
            _tournamentService.Verify(s => s.RunTournamentAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetStatistics_ReturnsBadRequest_WhenSortByIsBlank(string sortBy)
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: sortBy, sortDirection: "asc");

            Assert.Equal("sortBy parameter is required", GetErrorMessage(result));
        }

        [Fact]
        public async Task GetStatistics_ReturnsBadRequest_WhenSortByIsInvalid()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "type", sortDirection: "asc");

            Assert.Equal("sortBy parameter is invalid", GetErrorMessage(result));
            _tournamentService.Verify(s => s.RunTournamentAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetStatistics_ReturnsBadRequest_WhenSortDirectionIsInvalid()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "wins", sortDirection: "sideways");

            Assert.Equal("sortDirection parameter is invalid", GetErrorMessage(result));
            _tournamentService.Verify(s => s.RunTournamentAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("WINS")]
        [InlineData("Wins")]
        [InlineData("wins")]
        public async Task GetStatistics_AcceptsSortByCaseInsensitively(string sortBy)
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: sortBy, sortDirection: "asc");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetStatistics_SortsAscendingByWins_WhenRequested()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "wins", sortDirection: "asc");

            var dtos = Assert.IsAssignableFrom<IEnumerable<PokemonStatisticDto>>(
                Assert.IsType<OkObjectResult>(result).Value).ToList();

            Assert.Equal(new[] { "squirtle", "bulbasaur", "pikachu" }, dtos.Select(d => d.Name));
        }

        [Fact]
        public async Task GetStatistics_SortsDescendingByLosses_WhenRequested()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "losses", sortDirection: "desc");

            var dtos = Assert.IsAssignableFrom<IEnumerable<PokemonStatisticDto>>(
                Assert.IsType<OkObjectResult>(result).Value).ToList();

            Assert.Equal(new[] { "squirtle", "bulbasaur", "pikachu" }, dtos.Select(d => d.Name));
        }

        [Fact]
        public async Task GetStatistics_SortsByName_Alphabetically_WhenAscending()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "name", sortDirection: "asc");

            var dtos = Assert.IsAssignableFrom<IEnumerable<PokemonStatisticDto>>(
                Assert.IsType<OkObjectResult>(result).Value).ToList();

            Assert.Equal(new[] { "bulbasaur", "pikachu", "squirtle" }, dtos.Select(d => d.Name));
        }

        [Fact]
        public async Task GetStatistics_DefaultsToAscending_WhenSortDirectionIsOmitted()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "id");

            var dtos = Assert.IsAssignableFrom<IEnumerable<PokemonStatisticDto>>(
                Assert.IsType<OkObjectResult>(result).Value).ToList();

            Assert.Equal(new[] { 1, 7, 25 }, dtos.Select(d => d.Id));
        }

        [Fact]
        public async Task GetStatistics_MapsDomainStatisticsToDtos_WithLowercaseType()
        {
            var service = CreateService();

            var result = await service.GetStatistics(sortBy: "id", sortDirection: "asc");

            var dtos = Assert.IsAssignableFrom<IEnumerable<PokemonStatisticDto>>(
                Assert.IsType<OkObjectResult>(result).Value).ToList();

            var bulbasaur = dtos.Single(d => d.Id == 1);
            Assert.Equal("bulbasaur", bulbasaur.Name);
            Assert.Equal("grass", bulbasaur.Type);
            Assert.Equal(6, bulbasaur.Wins);
            Assert.Equal(6, bulbasaur.Losses);
            Assert.Equal(3, bulbasaur.Ties);
        }
    }
}

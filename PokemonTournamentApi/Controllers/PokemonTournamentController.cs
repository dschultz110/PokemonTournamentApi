using Microsoft.AspNetCore.Mvc;
using PokemonTournament.Core.Services;
using PokemonTournamentApi.Dtos;

namespace PokemonTournamentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonTournamentController : Controller
    {
        private static readonly HashSet<string> ValidSortByValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "wins", "losses", "ties", "name", "id"
        };

        private static readonly HashSet<string> ValidSortDirectionValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "asc", "desc"
        };

        private readonly ITournamentService _tournamentService;

        public PokemonTournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        [Route("/pokemon/tournament/statistics")]
        [ProducesResponseType(typeof(IEnumerable<PokemonStatisticDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return BadRequest(new { error = "sortBy parameter is required" });
            }

            if (!ValidSortByValues.Contains(sortBy))
            {
                return BadRequest(new { error = "sortBy parameter is invalid" });
            }

            if (!ValidSortDirectionValues.Contains(sortDirection))
            {
                return BadRequest(new { error = "sortDirection parameter is invalid" });
            }

            var statistics = await _tournamentService.RunTournamentAsync(cancellationToken);

            var dtos = statistics.Select(PokemonStatisticDto.FromDomain);

            var sorted = Sort(dtos, sortBy, sortDirection);

            return Ok(sorted);
        }

        private static IEnumerable<PokemonStatisticDto> Sort(
            IEnumerable<PokemonStatisticDto> statistics,
            string sortBy,
            string sortDirection)
        {
            var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

            Func<PokemonStatisticDto, object> keySelector = sortBy.ToLowerInvariant() switch
            {
                "wins" => s => s.Wins,
                "losses" => s => s.Losses,
                "ties" => s => s.Ties,
                "name" => s => s.Name,
                "id" => s => s.Id,
                _ => s => s.Id
            };

            return descending
                ? statistics.OrderByDescending(keySelector)
                : statistics.OrderBy(keySelector);
        }
    }
}

using PokemonTournament.Core.Models;

namespace PokemonTournamentApi.Dtos
{
    public class PokemonStatisticDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Ties { get; set; }

        public static PokemonStatisticDto FromDomain(PokemonStatistic statistic)
        {
            return new PokemonStatisticDto
            {
                Id = statistic.Pokemon.Id,
                Name = statistic.Pokemon.Name,
                Type = statistic.Pokemon.Type.ToString(),
                Wins = statistic.Wins,
                Losses = statistic.Losses,
                Ties = statistic.Ties
            };
        }
    }
}

namespace PokemonTournament.Core.Models
{
    public class PokemonStatistic(Pokemon pokemon)
    {
        public Pokemon Pokemon { get; } = pokemon;

        public int Wins { get; private set; }

        public int Losses { get; private set; }

        public int Ties { get; private set; }

        public void RecordWin() => Wins++;

        public void RecordLoss() => Losses++;

        public void RecordTie() => Ties++;
    }
}

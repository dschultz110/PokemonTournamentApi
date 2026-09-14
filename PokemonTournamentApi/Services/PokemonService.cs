namespace PokemonTournamentApi.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<List<Model.Pokemon>> GetAllPokemonsAsync()
        {
            throw new NotImplementedException();
        }
    }
}

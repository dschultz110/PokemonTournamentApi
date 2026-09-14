using System.Net.Http.Json;
using PokemonTournament.Core.Models;

namespace PokemonTournament.Core.Services
{
    public class PokeApiClient(HttpClient httpClient) : IPokeApiClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Pokemon> GetPokemonAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PokeApiPokemonResponse>($"{id}", cancellationToken)
                ?? throw new InvalidOperationException($"PokéAPI returned no data for Pokémon id {id}.");

            return Pokemon.FromApiResponse(response);
        }
    }
}

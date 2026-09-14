namespace PokemonTournamentApi.Services
{
    public interface IPokemonService
    {
        Task<List<Model.Pokemon>> GetAllPokemonsAsync();
    }
}

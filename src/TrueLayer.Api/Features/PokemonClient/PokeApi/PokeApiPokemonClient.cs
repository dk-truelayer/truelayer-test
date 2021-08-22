using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TrueLayer.Api.Features.PokemonClient.PokeApi.Models;
using TrueLayer.Api.Models;
using TrueLayer.Api.Utilities;

namespace TrueLayer.Api.Features.PokemonClient.PokeApi
{
    public class PokeApiPokemonClient : IPokemonClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<PokeApiOptions> _options;

        public PokeApiPokemonClient(IHttpClientFactory httpClientFactory, IOptions<PokeApiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public async Task<Pokemon?> GetPokemonWithName(string name)
        {
            Check.NotNull(name, nameof(name));
            
            var client = _httpClientFactory.CreateClient();

            var url = $"{_options.Value.BaseUrl}/pokemon-species/{name}";

            var pokemonResponse = await client.GetAsync(url);

            if (pokemonResponse.IsSuccessStatusCode)
            {
                var pokemonSpecies = await pokemonResponse
                    .Content
                    .ReadFromJsonAsync<PokemonSpecies>();

                if (pokemonSpecies is not null)
                {
                    var description = pokemonSpecies
                        .FlavorTextEntries
                        .First(entry => entry.Language.Name == "en")
                        .FlavorText;

                    return new Pokemon(
                        pokemonSpecies.Name,
                        description,
                        pokemonSpecies.Habitat.Name,
                        pokemonSpecies.IsLegendary);
                }
            }

            return null;
        }
    }
}
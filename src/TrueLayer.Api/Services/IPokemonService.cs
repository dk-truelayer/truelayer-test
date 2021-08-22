using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrueLayer.Api.Features.PokemonClient;
using TrueLayer.Api.Models;
using TrueLayer.Api.ViewModels;

namespace TrueLayer.Api.Services
{
    public interface IPokemonService
    {
        /// <summary>
        /// Gets information about a pokemon.
        /// </summary>
        /// <returns>Information about the pokemon, or null if it doesn't exist.</returns>
        Task<PokemonViewModel?> GetPokemonInformation(string name);

        /// <summary>
        /// Gets information about a pokemon, with the Description translated.
        /// </summary>
        /// <returns>Information about the pokemon, or null if it doesn't exist. The description has been translated.</returns>
        Task<PokemonViewModel?> GetTranslatedPokemonInformation(string name);
    }

    public class PokemonService : IPokemonService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPokemonClient _pokemonClient;

        public PokemonService(IHttpClientFactory httpClientFactory, IPokemonClient pokemonClient)
        {
            _httpClientFactory = httpClientFactory;
            _pokemonClient = pokemonClient;
        }

        public async Task<PokemonViewModel?> GetPokemonInformation(string name)
        {
            var pokemon = await GetUntranslatedPokemon(name);

            return ToViewModel(pokemon);
        }

        public async Task<PokemonViewModel?> GetTranslatedPokemonInformation(string name)
        {
            var pokemon = await GetUntranslatedPokemon(name);

            if (pokemon is null)
            {
                return null;
            }

            var pokemonTranslated = await Translate(pokemon);
            
            return ToViewModel(pokemonTranslated);
        }

        private Task<Pokemon?> GetUntranslatedPokemon(string name)
        {
            return _pokemonClient.GetPokemonWithName(name);
        }

        private Task<Pokemon> Translate(Pokemon pokemon)
        {
            return pokemon switch
            {
                {Habitat: "cave"} or {IsLegendary: true} => TranslateToYoda(pokemon),
                { } => TranslateToShakespeare(pokemon)
            };
        }

        private async Task<Pokemon> TranslateToYoda(Pokemon pokemon)
        {
            var newDescription = await TranslateText("yoda", pokemon.Description);
            
            return pokemon with
            {
                Description = newDescription ?? pokemon.Description
            };
        }

        private async Task<Pokemon> TranslateToShakespeare(Pokemon pokemon)
        {
            var newDescription = await TranslateText("shakespeare", pokemon.Description);
            
            return pokemon with
            {
                Description = newDescription ?? pokemon.Description
            };
        }

        private async Task<string?> TranslateText(string language, string text)
        {
            var client = _httpClientFactory.CreateClient();

            var form = new KeyValuePair<string, string>[]
            {
                new("text", text)
            };

            var content = new FormUrlEncodedContent(form);

            var response =
                await client.PostAsync($"https://api.funtranslations.com/translate/{language}.json", content);

            if (response.IsSuccessStatusCode)
            {
                var translateResult = await response
                    .Content
                    .ReadFromJsonAsync<TranslationResult>();

                return translateResult?.Contents.Translated;
            }

            return null;
        }

        private PokemonViewModel? ToViewModel(Pokemon? pokemon)
        {
            if (pokemon is null)
            {
                return null;
            }

            return new PokemonViewModel
            {
                Name = pokemon.Name,
                Description = pokemon.Description,
                Habitat = pokemon.Habitat,
                IsLegendary = pokemon.IsLegendary,
            };
        }

        public class TranslationResult
        {
            [JsonPropertyName("contents")]
            public TranslationContents Contents { get; set; }
        }
        
        public class TranslationContents
        {
            [JsonPropertyName("translated")]
            public string Translated { get; set; }
            
            [JsonPropertyName("text")]
            public string Text { get; set; }
            
            [JsonPropertyName("translation")]
            public string Translation { get; set; }
        }
    }
}
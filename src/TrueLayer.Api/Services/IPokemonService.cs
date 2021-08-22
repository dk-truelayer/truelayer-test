using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

        public PokemonService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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

        private async Task<Pokemon?> GetUntranslatedPokemon(string name)
        {
            var client = _httpClientFactory.CreateClient();

            var pokemonResponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{name}");

            if (pokemonResponse.IsSuccessStatusCode)
            {
                var pokemonSpecies = await pokemonResponse.Content.ReadFromJsonAsync<PokemonSpecies>();

                if (pokemonSpecies is { })
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

        public class PokemonSpecies
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            
            [JsonPropertyName("is_legendary")]
            public bool IsLegendary { get; set; }
            
            [JsonPropertyName("habitat")]
            public Habitat Habitat { get; set; }
            
            [JsonPropertyName("flavor_text_entries")]
            public List<FlavorTextEntry> FlavorTextEntries { get; set; }
        }

        public class Habitat
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class FlavorTextEntry
        {
            [JsonPropertyName("flavor_text")]
            public string FlavorText { get; set; }

            [JsonPropertyName("language")]
            public Language Language { get; set; }
        }

        public class Language
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
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
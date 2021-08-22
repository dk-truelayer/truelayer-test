using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrueLayer.Api.Features.PokemonCache;
using TrueLayer.Api.Features.PokemonClient;
using TrueLayer.Api.Features.Translation;
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
        private readonly IPokemonClient _pokemonClient;
        private readonly ITranslationClient _translationClient;
        private readonly IPokemonManager _pokemonManager;
        private readonly IPokemonCache _pokemonCache;

        public PokemonService(IPokemonClient pokemonClient, ITranslationClient translationClient, IPokemonManager pokemonManager, IPokemonCache pokemonCache)
        {
            _pokemonClient = pokemonClient;
            _translationClient = translationClient;
            _pokemonManager = pokemonManager;
            _pokemonCache = pokemonCache;
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
            if (_pokemonCache.Get(name) is {} cachedPokemon)
            {
                return cachedPokemon;
            }
            
            var pokemon = await _pokemonClient.GetPokemonWithName(name);

            if (pokemon is null)
            {
                return null;
            }
            
            _pokemonCache.Set(pokemon);

            return pokemon;
        }

        private async Task<Pokemon> Translate(Pokemon pokemon)
        {
            if (_pokemonCache.GetTranslated(pokemon.Name) is { } cachedPokemon)
            {
                return cachedPokemon;
            }
            
            var translationLanguage = _pokemonManager.ChooseTranslationLanguage(pokemon);

            var newDescription = await _translationClient.Translate(translationLanguage, pokemon.Description);

            var translatedPokemon = _pokemonManager.UpdateDescription(pokemon, newDescription);
            
            _pokemonCache.SetTranslated(translatedPokemon);

            return translatedPokemon;
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
    }
}
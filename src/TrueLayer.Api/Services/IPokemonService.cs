using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

        public PokemonService(IPokemonClient pokemonClient, ITranslationClient translationClient, IPokemonManager pokemonManager)
        {
            _pokemonClient = pokemonClient;
            _translationClient = translationClient;
            _pokemonManager = pokemonManager;
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

        private async Task<Pokemon> Translate(Pokemon pokemon)
        {
            var translationLanguage = _pokemonManager.ChooseTranslationLanguage(pokemon);

            var newDescription = await _translationClient.Translate(translationLanguage, pokemon.Description);

            return _pokemonManager.UpdateDescription(pokemon, newDescription);
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
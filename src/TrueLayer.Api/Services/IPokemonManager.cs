using System;
using TrueLayer.Api.Features.Translation;
using TrueLayer.Api.Models;
using TrueLayer.Api.ViewModels;

namespace TrueLayer.Api.Services
{
    public interface IPokemonManager
    {
        /// <summary>
        /// Chooses which language a pokemon's description should be translated into.
        /// </summary>
        TranslationLanguage ChooseTranslationLanguage(Pokemon pokemon);
        
        /// <summary>
        /// Returns a new pokemon with an updated description.
        /// </summary>
        Pokemon UpdateDescription(Pokemon pokemon, string? newDescription);

        /// <summary>
        /// Maps a pokemon to a viewmodel representation.
        /// </summary>
        PokemonViewModel? ToViewModel(Pokemon? pokemon);
    }
    
    public class PokemonManager : IPokemonManager
    {
        public TranslationLanguage ChooseTranslationLanguage(Pokemon pokemon)
        {
            return pokemon switch
            {
                {Habitat: "cave"} or {IsLegendary: true} => TranslationLanguage.Yoda,
                { } => TranslationLanguage.Shakespeare,
                _ => throw new ArgumentNullException(nameof(pokemon))
            };
        }

        public Pokemon UpdateDescription(Pokemon pokemon, string? newDescription)
        {
            return pokemon with
            {
                Description = newDescription ?? pokemon.Description
            };
        }

        public PokemonViewModel? ToViewModel(Pokemon? pokemon)
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
                IsLegendary = pokemon.IsLegendary
            };
        }
    }
}
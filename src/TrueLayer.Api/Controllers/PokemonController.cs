using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrueLayer.Api.Services;

namespace TrueLayer.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("{pokemonName:length(3,15)}")]
        public async Task<IActionResult> GetPokemonInformation(string pokemonName)
        {
            var pokemon = await _pokemonService.GetPokemonInformation(pokemonName);

            return pokemon switch
            {
                { } => Ok(pokemon),
                null => NotFound()
            };
        }

        [HttpGet("translated/{pokemonName:length(3,15)}")]
        public async Task<IActionResult> GetTranslatedPokemonInformation(string pokemonName)
        {
            var pokemon = await _pokemonService.GetTranslatedPokemonInformation(pokemonName);

            return pokemon switch
            {
                { } => Ok(pokemon),
                null => NotFound()
            };
        }
    }
}
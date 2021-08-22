using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TrueLayer.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokemonController : ControllerBase
    {
        [HttpGet("{pokemonName:length(3,15)}")]
        public async Task<IActionResult> GetPokemonInformation(string pokemonName)
        {
            throw new NotImplementedException();
        }

        [HttpGet("translated/{pokemonName:length(3,15)}")]
        public async Task<IActionResult> GetTranslatedPokemonInformation(string pokemonName)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueLayer.Api.Models;

namespace TrueLayer.Api.Features.PokemonClient.Dummy
{
    public class DummyPokemonClient : IPokemonClient
    {
        private static readonly Dictionary<string, Pokemon> Pokemons = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "mewtwo", new Pokemon("mewtwo", "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.", "rare", true) },
            { "bulbasaur", new Pokemon("bulbasaur", "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.", "grassland", false)},
        };

        public Task<Pokemon?> GetPokemonWithName(string name)
        {
            if (Pokemons.TryGetValue(name, out var pokemon))
            {
                return Task.FromResult<Pokemon?>(pokemon);
            }

            return Task.FromResult<Pokemon?>(null);
        }
    }
}
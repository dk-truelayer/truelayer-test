using System.Threading.Tasks;
using TrueLayer.Api.Models;

namespace TrueLayer.Api.Features.PokemonClient
{
    public interface IPokemonClient
    {
        /// <summary>
        /// Attempts to find a pokemon with a given name. If no such pokemon exists, returns null.
        /// </summary>
        Task<Pokemon?> GetPokemonWithName(string name);
    }
}
using TrueLayer.Api.Models;

namespace TrueLayer.Api.Features.PokemonCache.Dummy
{
    /// <summary>
    /// Cache implementation that does no caching. Everything
    /// is a no-op.
    /// </summary>
    public class DummyPokemonCache : IPokemonCache
    {
        public Pokemon? Get(string name)
        {
            return null;
        }

        public Pokemon? GetTranslated(string name)
        {
            return null;
        }

        public void Set(Pokemon pokemon)
        {
            // no-op
        }

        public void SetTranslated(Pokemon pokemon)
        {
            // no-op
        }
    }
}
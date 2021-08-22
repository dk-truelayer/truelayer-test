using TrueLayer.Api.Models;

namespace TrueLayer.Api.Features.PokemonCache
{
    /// <summary>
    /// A pokemon cache can store two copies of each pokemon. One copy is the raw
    /// pokemon, the other has potentially had a translation applied. Each value
    /// is updated separately.
    /// </summary>
    public interface IPokemonCache
    {
        /// <summary>
        /// Attempts to get a pokemon with a given name. If it isn't in the cache,
        /// returns null.
        /// </summary>
        Pokemon? Get(string name);
        
        /// <summary>
        /// Attempts to get a translated pokemon with a given name. If it isn't in
        /// the cache, returns null.
        /// </summary>
        Pokemon? GetTranslated(string name);

        /// <summary>
        /// Adds an untranslated pokemon to the cache. The pokemon's 'name' field
        /// is used as the key. If a pokemon already exists, it is overwritten. If
        /// a translated pokemon already exists, it is left untouched. 
        /// </summary>
        void Set(Pokemon pokemon);

        /// <summary>
        /// Adds a translated pokemon to the cache. The pokemon's 'name' field is
        /// used as the key. if a translated pokemon already exists with this name,
        /// it is overwritten. If an untranslated pokemon already exists, it is
        /// left untouched.
        /// </summary>
        void SetTranslated(Pokemon pokemon);
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using TrueLayer.Api.Models;

namespace TrueLayer.Api.Features.PokemonCache.Memory
{
    /// <summary>
    /// Totally in-memory cache. Using this cache means the storage is local to
    /// a node. 
    /// </summary>
    public class MemoryPokemonCache : IPokemonCache
    {
        record CacheEntry(Pokemon? Raw, Pokemon? Translated);

        private readonly IMemoryCache _cache;

        public MemoryPokemonCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Pokemon? Get(string name)
        {
            if (TryGetFromCache(name, out var cacheEntry) &&
                cacheEntry is { Raw: {} pokemon })
            {
                return pokemon;
            }

            return null;
        }

        public Pokemon? GetTranslated(string name)
        {
            if (TryGetFromCache(name, out var cacheEntry) &&
                cacheEntry is {Translated: { } pokemon})
            {
                return pokemon;
            }

            return null;
        }

        public void Set(Pokemon pokemon)
        {
            if (TryGetFromCache(pokemon.Name, out var cacheEntry))
            {
                var updatedEntry = cacheEntry with
                {
                    Raw = pokemon
                };
                
                SetCache(pokemon.Name, updatedEntry);
            }
            else
            {
                var newEntry = new CacheEntry(pokemon, null);
                
                SetCache(pokemon.Name, newEntry);
            }
        }

        public void SetTranslated(Pokemon pokemon)
        {
            if (TryGetFromCache(pokemon.Name, out var cacheEntry))
            {
                var updatedEntry = cacheEntry with
                {
                    Translated = pokemon
                };
                
                SetCache(pokemon.Name, updatedEntry);
            }
            else
            {
                var newEntry = new CacheEntry(null, pokemon);
                
                SetCache(pokemon.Name, newEntry);
            }
        }

        private bool TryGetFromCache(string name, [NotNullWhen(true)] out CacheEntry? cacheEntry)
        {
            return _cache.TryGetValue(name.ToLower(), out cacheEntry);
        }

        private void SetCache(string name, CacheEntry cacheEntry)
        {
            _cache.Set(name.ToLower(), cacheEntry, TimeSpan.FromDays(1));
        }
    }
}
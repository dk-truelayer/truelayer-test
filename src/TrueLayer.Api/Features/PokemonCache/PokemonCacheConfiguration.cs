using System;
using Microsoft.Extensions.Configuration;
using TrueLayer.Api.Features.PokemonCache;
using TrueLayer.Api.Features.PokemonCache.Dummy;
using TrueLayer.Api.Features.PokemonCache.Memory;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class PokemonCacheConfiguration
    {
        public static void ConfigurePokemonCache(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("Features:PokemonCache").Get<PokemonCacheOptions>();

            Type pokemonCacheImplementationType = options.Implementation switch
            {
                PokemonCacheOptions.PokemonCacheImplementation.Memory => typeof(MemoryPokemonCache),
                PokemonCacheOptions.PokemonCacheImplementation.Dummy => typeof(DummyPokemonCache),
                _ => throw new InvalidOperationException($"Implementation {configuration["Features:PokemonCache:Implementation"]} is not supported")
            };

            services.AddScoped(typeof(IPokemonCache), pokemonCacheImplementationType);
        }
    }
}
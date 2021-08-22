using System;
using Microsoft.Extensions.Configuration;
using TrueLayer.Api.Features.PokemonClient;
using TrueLayer.Api.Features.PokemonClient.Dummy;
using TrueLayer.Api.Features.PokemonClient.PokeApi;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class PokemonClientConfiguration
    {
        public static void ConfigurePokemonClient(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("Features:PokemonClient").Get<PokemonClientOptions>();

            var pokemonClientImplementationType = options.Implementation switch
            {
                PokemonClientOptions.PokemonClientImplementation.PokeApi => typeof(PokeApiPokemonClient),
                PokemonClientOptions.PokemonClientImplementation.Dummy => typeof(DummyPokemonClient),
                _ => throw new InvalidOperationException(
                    $"ConfigurePokemonClient: {configuration["Features:PokemonClient:Implementation"]} is not a valid implementation.")
            };

            services.AddScoped(typeof(IPokemonClient), pokemonClientImplementationType);

            services.Configure<PokeApiOptions>(configuration.GetSection("Features:PokemonClient:PokeApi"));
        }
    }
}
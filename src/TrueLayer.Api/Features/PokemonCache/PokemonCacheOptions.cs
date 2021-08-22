namespace TrueLayer.Api.Features.PokemonCache
{
    public class PokemonCacheOptions
    {
        public enum PokemonCacheImplementation
        {
            Dummy,
            Memory,
        }

        public PokemonCacheImplementation Implementation { get; set; }
    }
}
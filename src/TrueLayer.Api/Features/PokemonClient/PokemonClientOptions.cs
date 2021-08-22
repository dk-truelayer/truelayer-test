namespace TrueLayer.Api.Features.PokemonClient
{
    public class PokemonClientOptions
    {
        public enum PokemonClientImplementation
        {
            Dummy,
            PokeApi
        }

        public PokemonClientImplementation Implementation { get; set; }
    }
}
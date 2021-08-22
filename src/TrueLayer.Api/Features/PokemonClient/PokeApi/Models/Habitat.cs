using System.Text.Json.Serialization;

namespace TrueLayer.Api.Features.PokemonClient.PokeApi.Models
{
    public class Habitat
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
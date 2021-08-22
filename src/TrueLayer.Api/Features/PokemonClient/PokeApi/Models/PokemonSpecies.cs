using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrueLayer.Api.Services;

namespace TrueLayer.Api.Features.PokemonClient.PokeApi.Models
{
        public class PokemonSpecies
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            
            [JsonPropertyName("is_legendary")]
            public bool IsLegendary { get; set; }
            
            [JsonPropertyName("habitat")]
            public Habitat Habitat { get; set; }
            
            [JsonPropertyName("flavor_text_entries")]
            public List<FlavorTextEntry> FlavorTextEntries { get; set; }
        }
}
using System.Text.Json.Serialization;

namespace TrueLayer.Api.Features.Translation.FunTranslations.Models
{
    public class TranslationContents
    {
        [JsonPropertyName("translated")]
        public string Translated { get; set; }
            
        [JsonPropertyName("text")]
        public string Text { get; set; }
            
        [JsonPropertyName("translation")]
        public string Translation { get; set; }
    }
}
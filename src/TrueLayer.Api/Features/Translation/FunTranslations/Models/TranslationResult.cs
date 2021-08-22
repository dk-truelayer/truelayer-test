using System.Text.Json.Serialization;

namespace TrueLayer.Api.Features.Translation.FunTranslations.Models
{
        public class TranslationResult
        {
            [JsonPropertyName("contents")]
            public TranslationContents Contents { get; set; }
        }
}
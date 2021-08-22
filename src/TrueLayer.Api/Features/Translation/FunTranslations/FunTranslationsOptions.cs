using System.Collections.Generic;

namespace TrueLayer.Api.Features.Translation.FunTranslations
{
    public class FunTranslationsOptions
    {
        public string BaseUrl { get; set; }

        public Dictionary<string, string> Endpoints { get; set; }
    }
}
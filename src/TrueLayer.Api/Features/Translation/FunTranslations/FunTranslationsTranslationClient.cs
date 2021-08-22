using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TrueLayer.Api.Features.Translation.FunTranslations.Models;

namespace TrueLayer.Api.Features.Translation.FunTranslations
{
    public class FunTranslationsTranslationClient : ITranslationClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FunTranslationsOptions> _options;

        public FunTranslationsTranslationClient(IHttpClientFactory httpClientFactory, IOptions<FunTranslationsOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public Task<string?> Translate(TranslationLanguage language, string text)
        {
            return language switch
            {
                TranslationLanguage.None => TranslateToNone(text),
                TranslationLanguage.Shakespeare => TranslateToShakespeare(text),
                TranslationLanguage.Yoda => TranslateToYoda(text),
                _ => Task.FromResult<string?>(null)
            };
        }

        private Task<string?> TranslateToNone(string text) => Task.FromResult<string?>(text);

        private Task<string?> TranslateToShakespeare(string text) =>
            PerformTranslation(_options.Value.Endpoints["Shakespeare"], text);
        
        private Task<string?> TranslateToYoda(string text) =>
            PerformTranslation(_options.Value.Endpoints["Yoda"], text);

        private async Task<string?> PerformTranslation(string endpoint, string text)
        {
            var client = _httpClientFactory.CreateClient();

            var url = $"{_options.Value.BaseUrl}/{endpoint}";

            var requestContent = MakeRequestContent(text);

            var translationResponse = await client.PostAsync(url, requestContent);

            if (translationResponse.IsSuccessStatusCode)
            {
                var translationResult = await translationResponse
                    .Content
                    .ReadFromJsonAsync<TranslationResult>();

                if (translationResult is not null)
                {
                    return translationResult.Contents.Translated;
                }
            }

            return null;
        }

        private HttpContent MakeRequestContent(string text)
        {
            var form = new KeyValuePair<string, string>[]
            {
                new("text", text)
            };

            return new FormUrlEncodedContent(form!);
        }
    }
}
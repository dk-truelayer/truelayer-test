using System.Threading.Tasks;

namespace TrueLayer.Api.Features.Translation.Dummy
{
    public class DummyTranslationClient : ITranslationClient
    {
        public Task<string?> Translate(TranslationLanguage language, string text)
        {
            return Task.FromResult<string?>($"<{language:G}> {text}");
        }
    }
}
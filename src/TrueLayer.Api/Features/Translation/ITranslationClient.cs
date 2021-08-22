using System.Threading.Tasks;

namespace TrueLayer.Api.Features.Translation
{
    public interface ITranslationClient
    {
        /// <summary>
        /// Translates the text into the given language.
        ///
        /// If this fails, for any reason, then it returns null.
        /// </summary>
        Task<string?> Translate(TranslationLanguage language, string text);
    }
}
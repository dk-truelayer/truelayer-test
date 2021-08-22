namespace TrueLayer.Api.Features.Translation
{
    public class TranslationOptions
    {
        public enum TranslationImplementation
        {
            Dummy,
            FunTranslations,
        }

        public TranslationImplementation Implementation { get; set; }
    }
}
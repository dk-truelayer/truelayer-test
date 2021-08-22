using System;
using Microsoft.Extensions.Configuration;
using TrueLayer.Api.Features.Translation;
using TrueLayer.Api.Features.Translation.Dummy;
using TrueLayer.Api.Features.Translation.FunTranslations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TranslationConfiguration
    {
        public static void ConfigureTranslation(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("Features:Translation").Get<TranslationOptions>();

            var translationClientImplementationType = options.Implementation switch
            {
                TranslationOptions.TranslationImplementation.FunTranslations =>
                    typeof(FunTranslationsTranslationClient),
                TranslationOptions.TranslationImplementation.Dummy => typeof(DummyTranslationClient),
                _ => throw new InvalidOperationException(
                    $"Implementation {configuration["Features:Translation:Implementation"]} is not supported")
            };

            services.AddScoped(typeof(ITranslationClient), translationClientImplementationType);

            services.Configure<FunTranslationsOptions>(
                configuration.GetSection("Features:Translation:FunTranslations"));
        }
    }
}
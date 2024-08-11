using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace MultiLanguage.Api.Language
{
    public static class LanguageExtension
    {

        public static IServiceCollection AddLanguageServices(this IServiceCollection services)
        {

            services.AddLocalization();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            
            return services;

        }

        public static WebApplication UseLanguageServices(this WebApplication app)
        {

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"))
            };
            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            app.UseMiddleware<LocalizationMiddleware>();

            return app;
        }

    }

}

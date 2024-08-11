using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace MultiLanguage.Api.Language
{

    public class LocalizationMiddleware : IMiddleware
    {

        ///<inheritdoc/>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            StringValues cultureKey = context.Request.Headers["Accept-Language"];
            if (!string.IsNullOrEmpty(cultureKey) && DoesCultureExist(cultureKey))
            {
                CultureInfo culture = new(cultureKey);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            await next(context);

        }

        private static bool DoesCultureExist(string cultureName)
        {
            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Any(culture =>
                    string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));
        }
    }

}

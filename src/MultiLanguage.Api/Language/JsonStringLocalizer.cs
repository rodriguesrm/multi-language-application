using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace MultiLanguage.Api.Language
{
    public class JsonStringLocalizer : IStringLocalizer
    {

        #region Local Objects/Variables

        private readonly IDistributedCache _cache;

        #endregion

        #region Constructors

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }

        #endregion

        #region Local Methods

        private static string? GetValueFromJSON(string propertyName, string filePath)
        {
            if (propertyName == null)
                return default;
            
            if (filePath == null)
                return default;

            byte[] data = File.ReadAllBytes(filePath);
            var reader = new Utf8JsonReader(data);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == propertyName)
                {
                    reader.Read();
                    return reader.GetString();
                }
            }
            return default;
        }

        private string? GetString(string key)
        {
            string? relativeFilePath = $"Language/Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath = Path.GetFullPath(relativeFilePath);
            if (File.Exists(fullFilePath))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var result = GetValueFromJSON(key, Path.GetFullPath(relativeFilePath));

                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cacheKey, result);

                }
                return result;
            }
            return default;
        }

        #endregion

        #region Public methods

        ///<inheritdoc/>
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        ///<inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var currentValue = this[name];
                return !currentValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(currentValue.Value, arguments), false)
                    : currentValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {

            IList<LocalizedString> result = new List<LocalizedString>();

            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            byte[] data = File.ReadAllBytes(filePath);
            Utf8JsonReader reader = new(data);

            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                    continue;
                string key = reader.GetString() ?? string.Empty;
                reader.Read();
                string value = reader.GetString() ?? string.Empty;
                result.Add(new LocalizedString(key, value, false));
            }

            return result;


        }

        #endregion

    }
}

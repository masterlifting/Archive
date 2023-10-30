using System.Text.Json;

namespace dashboard.Services;

public static class DashboardHelpers
{
    public static class Json
    {
        public static Dictionary<string, string> ParseProperty(string key, string value)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(value))
            {
                result.Add(key, string.Empty);
                return result;
            }

            JsonDocument jsonDocument;
            
            try
            {
                jsonDocument = JsonDocument.Parse(value);
            }
            catch (Exception)
            {
                result.Add(key, value);
                return result;
            }

            if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
            {
                var dictionary = jsonDocument.RootElement.Deserialize<Dictionary<string, object>>() ?? throw new NotSupportedException();

                foreach (var item in dictionary.Where(x => x.Value is not null && x.Value is JsonElement))
                    ParseElement(item.Key, (JsonElement)item.Value);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(value))
                    result.Add(key, string.Empty);
                else
                    result.Add(key, value);
            }

            return result;

            void ParseElement(string key, JsonElement value)
            {
                switch (value.ValueKind)
                {
                    case JsonValueKind.Array:
                        {
                            var collection = value.Deserialize<string[]>() ?? throw new NotSupportedException();

                            for (var i = 0; i < collection.Length; i++)
                            {
                                if (string.IsNullOrWhiteSpace(collection[i]))
                                    continue;

                                result.Add(key, collection[i]);
                            }

                            break;
                        }
                    case JsonValueKind.Object:
                        {
                            var dictionary = value.Deserialize<Dictionary<string, object>>() ?? throw new NotSupportedException();

                            foreach (var item in dictionary.Where(x => x.Value is not null && x.Value is JsonElement))
                                ParseElement(key + " - " + item.Key, (JsonElement)item.Value);

                            break;
                        }
                    default:
                        {
                            result.Add(key, value.ToString());
                            break;
                        }
                }
            }
        }
    }
}

using Application.Pagination;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Presentation.JsonConverters;

internal class PageConverter<T> : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        Page<T> page = (Page<T>)value!;
        JObject jObject = new()
        {
            ["Elements"] = JToken.FromObject(page.Elements, serializer),
            ["TotalElements"] = page.TotalElements,
            ["TotalPages"] = page.TotalPages,
            ["IsEmpty"] = page.IsEmpty,
            ["IsLastPage"] = page.IsLastPage
        };
        
        jObject.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        Log.Information("Performing ReadJson operation in PageConverter.");
        
        JObject jObject = JObject.Load(reader);
        IEnumerable<T>? elements = jObject["Elements"].ToObject<List<T>>(serializer);
        
        bool isEmpty = jObject["IsEmpty"].ToObject<bool>(serializer);
        int totalPages = jObject["TotalPages"].ToObject<int>(serializer);
        int totalElements = jObject["TotalElements"].ToObject<int>(serializer);
        bool isLastPage = jObject["IsLastPage"].ToObject<bool>(serializer);
        
        return new Page<T>(elements, totalElements, totalPages, isEmpty, isLastPage);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Page<>);
    }
    
    public static JsonSerializerSettings GetSettings()
    {
        return new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PageConverter<T>()}
        };
    }
}
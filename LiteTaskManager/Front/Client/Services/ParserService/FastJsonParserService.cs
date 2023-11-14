using AppInfrastructure.Services.ParserService;

namespace Client.Services.ParserService;

/// <summary>
///     Преобразует данные из JSON в объекты и обратно
/// </summary>
public class FastJsonParserService : IParserService
{
    public string Serialize<T>(T? nonSerialized) => fastJSON.JSON.ToNiceJSON(nonSerialized);

    public T? DeSerialize<T>(string serialized) => fastJSON.JSON.ToObject<T>(serialized);
}
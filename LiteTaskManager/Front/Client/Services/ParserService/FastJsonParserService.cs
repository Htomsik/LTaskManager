using AppInfrastructure.Services.ParserService;

namespace Client.Services.ParserService;

public class FastJsonParserService : IParserService
{
    public string Serialize<T>(T? nonSerialized) => fastJSON.JSON.ToNiceJSON(nonSerialized);

    public T? DeSerialize<T>(string serialized) => fastJSON.JSON.ToObject<T>(serialized);
}
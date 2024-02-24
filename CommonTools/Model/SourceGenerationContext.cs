using System.Text.Json.Serialization;

namespace CommonTools.Model;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<int, string>))]
[JsonSerializable(typeof(Dictionary<char, string>))]
internal partial class SourceGenerationContext : JsonSerializerContext { }


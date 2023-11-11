using System.Text.Json.Serialization;

namespace MALibria.Anilibria;

struct Response
{
  [JsonPropertyName("list")]
  public required IEnumerable<Title> List { get; init; }
}
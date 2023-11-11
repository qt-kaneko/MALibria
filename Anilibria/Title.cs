using System.Text.Json.Serialization;

namespace MALibria.Anilibria;

struct Title
{
  [JsonPropertyName("id")]
  public required int Id { get; init; }

  [JsonPropertyName("player")]
  public required Player Player { get; init; }
}
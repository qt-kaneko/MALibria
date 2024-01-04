using System.Text.Json.Serialization;

namespace MALibria.Models.Anilibria;

struct Title
{
  [JsonPropertyName("id")]
  public required int Id { get; init; }

  [JsonPropertyName("player")]
  public required Player Player { get; init; }
}
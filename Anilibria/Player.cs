using System.Text.Json.Serialization;

namespace MALibria.Anilibria;

struct Player
{
  [JsonPropertyName("alternative_player")]
  public required string AlternativePlayer { get; init; }
}
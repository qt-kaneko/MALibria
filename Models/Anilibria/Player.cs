using System.Text.Json.Serialization;

namespace MALibria.Models.Anilibria;

struct Player
{
  [JsonPropertyName("alternative_player")]
  public required string AlternativePlayer { get; init; }

  [JsonPropertyName("episodes")]
  public required Episodes Episodes { get; init; }
}
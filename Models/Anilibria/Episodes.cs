using System.Text.Json.Serialization;

namespace MALibria.Models.Anilibria;

struct Episodes
{
  [JsonPropertyName("last")]
  public required int? Last { get; init; }
}
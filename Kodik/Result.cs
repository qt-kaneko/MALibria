using System.Text.Json.Serialization;

namespace MALibria.Kodik;

struct Result
{
  [JsonPropertyName("id")]
  public required string Id { get; init; }

  [JsonPropertyName("shikimori_id")]
  public string? ShikimoriId { get; init; }
}
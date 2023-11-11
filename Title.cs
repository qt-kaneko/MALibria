using System.Text.Json.Serialization;

struct Title
{
  [JsonPropertyName("anilibria_id")]
  public required int AnilibriaId { get; init; }

  [JsonPropertyName("myanimelist_id")]
  public required int MyAnimeListId { get; init; }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MALibria.Models.Anilibria;

struct Response
{
  [JsonPropertyName("list")]
  public required IEnumerable<Title> List { get; init; }
}
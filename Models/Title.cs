using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MALibria.Models;

struct Title : IEquatable<Title>
{
  [JsonPropertyName("anilibria_id")]
  public required int AnilibriaId { get; init; }

  [JsonPropertyName("myanimelist_id")]
  public required int MyAnimeListId { get; init; }

  [JsonPropertyName("episodes")]
  public required int Episodes { get; init; }

  public bool Equals(Title other)
  {
    return other.AnilibriaId == AnilibriaId
        && other.MyAnimeListId == MyAnimeListId
        && other.Episodes == Episodes;
  }
  public override bool Equals([NotNullWhen(true)] object? obj)
  {
    return (obj is Title title) && Equals(title);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(AnilibriaId, MyAnimeListId, Episodes);
  }

  public static bool operator ==(Title a, Title b) => a.Equals(b);
  public static bool operator !=(Title a, Title b) => !(a == b);
}
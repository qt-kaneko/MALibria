using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MALibria.Extensions;
using MALibria.Models;

using Anilibria = MALibria.Models.Anilibria;
using Kodik = MALibria.Models.Kodik;

namespace MALibria;

static partial class Program
{
  [GeneratedRegex("[0-9]+(?=/[0-9A-z]+)")]
  private static partial Regex _kodikIdRegex();

  static HttpClient _http = new(new HttpClientHandler() {
    AutomaticDecompression = System.Net.DecompressionMethods.All
  });

  static async Task Main()
  {
    var manuallyMappedTitles = ReadManullyMappedTitles();

    Console.WriteLine("Downloading Anilibria database...");
    (var anilibriaTitles, var anilibriaJson) = await FetchAnilibriaTitles();
    File.WriteAllText("anilibria.json", anilibriaJson);

    Console.WriteLine("Downloading Kodik database...");
    (var kodikTitles, var kodikSerialsJson, var kodikFilmsJson) = await FetchKodikTitles();
    File.WriteAllText("kodik-serials.json", kodikSerialsJson);
    File.WriteAllText("kodik-films.json", kodikFilmsJson);

    Console.WriteLine("Processing...");
    var mapped = anilibriaTitles.SelectWhere(anilibriaTitle => {
      var kodikIdMatch = _kodikIdRegex().Match(anilibriaTitle.Player.AlternativePlayer);
      if (!kodikIdMatch.Success) return null;

      var kodikId = kodikIdMatch.Value;

      var kodikTitle = kodikTitles.FirstOrDefault(kodik => Regex.IsMatch(kodik.Id, $"[A-z]+-{kodikId}$"));
      if (kodikTitle.ShikimoriId == null) return null;

      return (Title?)new Title() {
        AnilibriaId = anilibriaTitle.Id,
        MyAnimeListId = int.Parse(kodikTitle.ShikimoriId)
      };
    }).Union(manuallyMappedTitles)
      .ToArray();

    File.WriteAllText("mapped.json", JsonSerializer.Serialize(mapped));
  }

  static Title[] ReadManullyMappedTitles()
  {
    var titlesString = File.ReadAllText("ManuallyMapped.json");
    var titles = JsonSerializer.Deserialize<Title[]>(titlesString)!;

    return titles;
  }

  static async Task<(IEnumerable<Anilibria.Title>, string json)> FetchAnilibriaTitles()
  {
    var aggregateException = new AggregateException();
    for (var attempt = 1; attempt <= 10; ++attempt)
    {
      try
      {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.anilibria.tv/v3/title/search/advanced?query=exists({player.alternative_player})&filter=id,player.alternative_player&items_per_page=999999");
        var response = await _http.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[{attempt}] Anilibria response: {responseString.Substring(0, 75)}...");

        var titles = JsonSerializer.Deserialize<Anilibria.Response>(responseString)!
                                   .List;

        return (titles, responseString);
      }
      catch (Exception ex)
      {
        aggregateException = new AggregateException(aggregateException.InnerExceptions.Append(ex));
      }
    }

    throw aggregateException;
  }

  static async Task<(IEnumerable<Kodik.Result>, string serialsJson, string filmsString)>
    FetchKodikTitles()
  {
    var serialsRequest = new HttpRequestMessage(HttpMethod.Get, "https://dumps.kodik.biz/serials/anime-serial.json");
    var serialsResponse = _http.SendAsync(serialsRequest);

    var filmsRequest = new HttpRequestMessage(HttpMethod.Get, "https://dumps.kodik.biz/films/anime.json");
    var filmsResponse = _http.SendAsync(filmsRequest);

    await Task.WhenAll(serialsResponse, filmsResponse);

    var serialsString = await serialsResponse.Result.Content.ReadAsStringAsync();
    var serials = JsonSerializer.Deserialize<Kodik.Result[]>(serialsString)!;

    var filmsString = await filmsResponse.Result.Content.ReadAsStringAsync();
    var films = JsonSerializer.Deserialize<Kodik.Result[]>(filmsString)!;

    var titles = Enumerable.Concat(serials, films);

    return (titles, serialsString, filmsString);
  }
}
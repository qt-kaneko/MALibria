using System.Text.Json;
using System.Text.RegularExpressions;

using MALibria.Extensions;

namespace MALibria.Kodik;

static partial class Program
{
  [GeneratedRegex("[0-9]+(?=/[0-9A-z]+)")]
  private static partial Regex _kodikIdRegex();

  static HttpClient _http = new(new HttpClientHandler() {
    AutomaticDecompression = System.Net.DecompressionMethods.All
  });

  static async Task Main()
  {
    Console.WriteLine("Downloading Anilibria database...");
    var anilibriaTitles = await FetchAnilibriaTitles();

    Console.WriteLine("Downloading Kodik database...");
    var kodikTitles = await FetchKodikTitles();

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
    }).ToArray();

    File.WriteAllText("mapped.json", JsonSerializer.Serialize(mapped));
  }

  static async Task<IEnumerable<Anilibria.Title>> FetchAnilibriaTitles()
  {
    var anilibriaRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.anilibria.tv/v3/title/search/advanced?query=exists({player.alternative_player})&filter=id,player.alternative_player&items_per_page=999999");
    var anilibriaResponse = await _http.SendAsync(anilibriaRequest);
    var anilibriaText = await anilibriaResponse.Content.ReadAsStringAsync();
    var anilibriaTitles = JsonSerializer.Deserialize<Anilibria.Response>(anilibriaText)!
                                        .List;

    return anilibriaTitles;
  }

  static async Task<IEnumerable<Kodik.Result>> FetchKodikTitles()
  {
    var kodikSerialsRequest = new HttpRequestMessage(HttpMethod.Get, "https://dumps.kodik.biz/serials/anime-serial.json");
    var kodikSerialsResponse = _http.SendAsync(kodikSerialsRequest);

    var kodikFilmsRequest = new HttpRequestMessage(HttpMethod.Get, "https://dumps.kodik.biz/films/anime.json");
    var kodikFilmsResponse = _http.SendAsync(kodikFilmsRequest);

    await Task.WhenAll(kodikSerialsResponse, kodikFilmsResponse);

    var kodikSerialsText = await kodikSerialsResponse.Result.Content.ReadAsStringAsync();
    var kodikSerials = JsonSerializer.Deserialize<Kodik.Result[]>(kodikSerialsText)!;

    var kodikFilmsText = await kodikSerialsResponse.Result.Content.ReadAsStringAsync();
    var kodikFilms = JsonSerializer.Deserialize<Kodik.Result[]>(kodikFilmsText)!;

    var kodikTitles = Enumerable.Concat(kodikSerials, kodikFilms);

    return kodikTitles;
  }
}
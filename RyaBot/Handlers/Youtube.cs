using RyaBot.Models;
using RyaBot.Processes;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RyaBot.Handlers
{
  public class Youtube
  {
    public async Task<Song> Download(string url)
    {
      using (var ytDownloader = new YoutubeDl())
      {
        var data = (await ytDownloader.GetDataAsync(url)).Split('\n');
        if (data.Length < 6)
          return null;

        if (!TimeSpan.TryParseExact(data[4], new[] { "ss", "m\\:ss", "mm\\:ss", "h\\:mm\\:ss", "hh\\:mm\\:ss", "hhh\\:mm\\:ss" }, CultureInfo.InvariantCulture, out var time))
          time = TimeSpan.FromHours(24);

        if (time.TotalMinutes > 10)
          return null;

        return new Song
        {
          Title = data[0],
          Duration = time,
          Url = data[2],
        };
      }
    }

    public async Task<bool> GetYoutubeSong(string url, Settings settings)
    {
      var song = await Download(url);

      if (song == null || settings.PlayList.Contains(song))
        return false;
      
      settings.PlayList.Add(song);
      return true;
    }
  }
}
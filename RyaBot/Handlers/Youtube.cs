using RyaBot.Models;
using RyaBot.Services;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;

namespace RyaBot.Handlers
{
  public class Youtube
  {
    private readonly string _outputFolder = $"{Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar}music";

    public Youtube()
    {
    }

    public async Task<bool> Download(string url, Settings settings)
    {
      var ytClient = new YoutubeClient(); ;

      if (!YoutubeClient.TryParseVideoId(url, out string id))
        id = url;

      var exists = await ytClient.CheckVideoExistsAsync(id);

      if (exists)
      {
        Directory.CreateDirectory(_outputFolder);

        var videoInfo = await ytClient.GetVideoInfoAsync(id);

        if (videoInfo.Duration > TimeSpan.FromMinutes(10))
        {
          ytClient = null;
          return false;
        }

        var streamInfo = videoInfo.AudioStreams.OrderBy(s => s.AudioEncoding).Last();
        var fileExtension = streamInfo.Container.GetFileExtension();

        var filePath = Path.Combine(_outputFolder, await GetMD5(videoInfo.Title) + fileExtension);

        if (!File.Exists(filePath)) await ytClient.DownloadMediaStreamAsync(streamInfo, filePath);

        ytClient = null;

        var song = new Song { Path = filePath, Title = videoInfo.Title, Duration = videoInfo.Duration };

        if (!settings.playList.Contains(song))
        {
          settings.playList.Add(song);
          return true;
        }
      }
      ytClient = null;

      return false;
    }

    private async Task<string> GetMD5(string inputString)
    {
      var encodedPassword = new UTF8Encoding().GetBytes(inputString);
      var hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
      var encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
      return await Task.FromResult<string>(encoded);
    }
  }
}

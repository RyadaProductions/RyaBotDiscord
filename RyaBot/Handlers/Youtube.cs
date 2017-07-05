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
    private YoutubeClient ytClient;

    private string outputFolder = $"{Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar}videos";

    public Youtube()
    {
      ytClient = new YoutubeClient();
    }

    public async Task<bool> Download(string URL, Settings settings)
    {
      var exists = await ytClient.CheckVideoExistsAsync(URL);

      if (exists)
      {
        Console.WriteLine($"Opening / Creating output folder at: {outputFolder}");
        Directory.CreateDirectory(outputFolder);
        var videoInfo = await ytClient.GetVideoInfoAsync(URL);

        if (videoInfo.Duration > TimeSpan.FromMinutes(10)) return false;

        var streamInfo = videoInfo.AudioStreams.OrderBy(s => s.AudioEncoding).Last();
        var fileExtension = streamInfo.Container.GetFileExtension();

        var filePath = Path.Combine(outputFolder, await GetMD5(videoInfo.Title) + fileExtension);

        Console.WriteLine($"Trying to find song at path: {filePath}");

        if (!File.Exists(filePath)) await ytClient.DownloadMediaStreamAsync(streamInfo, filePath);

        settings.playList.TryAdd(filePath, videoInfo);
        return true;
      }
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

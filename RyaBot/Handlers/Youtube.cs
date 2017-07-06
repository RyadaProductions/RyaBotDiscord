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
    private readonly YoutubeClient _ytClient;
    private readonly string _outputFolder = $"{Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar}videos";

    public Youtube()
    {
      _ytClient = new YoutubeClient();
    }

    public async Task<bool> Download(string URL, Settings settings)
    {
      var exists = await _ytClient.CheckVideoExistsAsync(URL);

      if (exists)
      {
        Console.WriteLine($"Opening / Creating output folder at: {_outputFolder}");
        Directory.CreateDirectory(_outputFolder);
        var videoInfo = await _ytClient.GetVideoInfoAsync(URL);

        if (videoInfo.Duration > TimeSpan.FromMinutes(10)) return false;

        var streamInfo = videoInfo.AudioStreams.OrderBy(s => s.AudioEncoding).Last();
        var fileExtension = streamInfo.Container.GetFileExtension();

        var filePath = Path.Combine(_outputFolder, await GetMD5(videoInfo.Title) + fileExtension);

        Console.WriteLine($"Trying to find song at path: {filePath}");

        if (!File.Exists(filePath)) await _ytClient.DownloadMediaStreamAsync(streamInfo, filePath);

        if (settings.playList.Add(new Song { Path = filePath, Title = videoInfo.Title, Duration = videoInfo.Duration })) return true;
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

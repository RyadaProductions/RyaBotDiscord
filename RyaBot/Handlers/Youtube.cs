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
    private YoutubeClient downloader;

    private string outputFolder = $"{Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar}videos";
    //private string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";

    public Youtube()
    {
      downloader = new YoutubeClient();
    }

    public async Task<bool> Download(string URL, Settings settings)
    {
      bool exists = await downloader.CheckVideoExistsAsync(URL);
      if (exists)
      {
        Console.WriteLine($"Opening / Creating output folder at: {outputFolder}");
        Directory.CreateDirectory(outputFolder);
        VideoInfo videoInfo = await downloader.GetVideoInfoAsync(URL);
        if (videoInfo.Duration > TimeSpan.FromMinutes(10))
        {
          return false;
        }
        try
        {
          var streamInfo = videoInfo.AudioStreams.OrderBy(s => s.AudioEncoding).Last();
          string fileExtension = streamInfo.Container.GetFileExtension();

          string filePath = Path.Combine(outputFolder, await GetMD5(videoInfo.Title) + fileExtension);
        
        Console.WriteLine($"Trying to find song at path: {filePath}");
        if (!File.Exists(filePath))
        {
          await downloader.DownloadMediaStreamAsync(streamInfo, filePath);
        }
        settings.playList.TryAdd(filePath, videoInfo);
        return true;
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
      return false;
    }

    private async Task<string> GetMD5(string inputString)
    {
      byte[] encodedPassword = new UTF8Encoding().GetBytes(inputString);
      byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
      string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
      return await Task.FromResult<string>(encoded);
    }
  }
}

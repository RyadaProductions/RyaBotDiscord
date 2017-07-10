using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyaBot.Processes
{
  class YoutubeDL : IDisposable
  {
    public YoutubeDL()
    {
    }

    public async Task<string> GetDataAsync(string url)
    {
      using (Process process = new Process() {
        StartInfo = new ProcessStartInfo() {
          FileName = "3rd_party\\youtube-dl",
          Arguments = $"-f bestaudio -e --get-url --get-id --get-thumbnail --get-duration --no-check-certificate \"ytsearch:{url}\"",
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
        },
      })
      {
        process.Start();
        var str = await process.StandardOutput.ReadToEndAsync();
        var err = await process.StandardError.ReadToEndAsync();
        if (!string.IsNullOrEmpty(err))
          Console.WriteLine(err);
        return str;
      }
    }

    public void Dispose()
    {

    }
  }
}

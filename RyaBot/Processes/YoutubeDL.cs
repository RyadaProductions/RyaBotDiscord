﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RyaBot.Processes
{
  internal class YoutubeDl : IDisposable
  {
    public async Task<string> GetDataAsync(string url)
    {
      using (var process = new Process()
      {
        StartInfo = new ProcessStartInfo()
        {
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
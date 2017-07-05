using Discord.Audio;
using RyaBot.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Models;

namespace RyaBot.Processes
{
  public class Media
  {
    Settings settings;
    Msg message;
    private bool playing = false;
    public System.Timers.Timer timer;

    CancellationTokenSource source;

    public Media(Settings settings, Msg message)
    {
      this.settings = settings;
      this.message = message;

      timer = new System.Timers.Timer(1000);
      timer.Elapsed += async (sender, e) => await HandleTimerAsync();
      timer.Start();
    }

    private Process CreateStream(string path)
    {
      var ffmpeg = new ProcessStartInfo {
        FileName = "ffmpeg",
        Arguments = $"-loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        RedirectStandardOutput = true,
      };
      return Process.Start(ffmpeg);
    }

    public async Task StartStreamAsync(IAudioClient client, string mediaPath)
    {
      var ffmpegProcess = CreateStream(mediaPath);
      var ffmpegOutput = ffmpegProcess.StandardOutput.BaseStream;
      var discordAudioStream = client.CreatePCMStream(AudioApplication.Mixed);

      source = new CancellationTokenSource();
      Console.WriteLine("new " + source);

      await ffmpegOutput.CopyToAsync(discordAudioStream, 81920, source.Token).ContinueWith(task => {
        if (!task.IsCanceled && task.IsFaulted) //supress cancel exception
          Console.WriteLine(task.Exception);
      });
      ffmpegProcess.WaitForExit();
      await discordAudioStream.FlushAsync();

      Console.WriteLine("dispose " + source);
      source.Dispose();
      settings.currentSong = "";
      playing = false;
    }

    public async Task StopStreamAsync()
    {
      if (playing)
      {
        source.Cancel();
        timer.Stop();
        
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += async (sender, e) => await HandleTimerAsync();
        timer.Start();

        playing = false;
      }
    }

    private async Task HandleTimerAsync()
    {
      try
      {
        if (settings.voiceClient != null && settings.playList.Count > 0 && !playing)
        {
          playing = true;
          string path = settings.playList.Keys.First();

          settings.playList.TryRemove(settings.playList.Keys.First(), out VideoInfo video);
          await message.SendToChannel(331741897737502720, $"Now playing: {video.Title}");
          settings.currentSong = video.Title;
          await StartStreamAsync(settings.voiceClient, path);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }
  }
}

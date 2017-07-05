using Discord.Audio;
using RyaBot.Handlers;
using RyaBot.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Models;

namespace RyaBot.Processes
{
  public class Media : IDisposable
  {
    private readonly Settings _settings;
    private readonly Message _message;

    private bool _playing = false;
    private CancellationTokenSource _source;
    private System.Timers.Timer _timer;


    public Media(Settings settings, Message message)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(Settings));
      _message = message ?? throw new ArgumentNullException(nameof(Message));

      _timer = new System.Timers.Timer(1000);
      _timer.AutoReset = false;
      _timer.Elapsed += async (sender, e) => { await HandleTimerAsync(); _timer.Start(); };
      _timer.Start();
    }

    private Process CreateStream(string path)
    {
      var ffmpeg = new ProcessStartInfo {
        FileName = "3rd_party\\ffmpeg",
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

      _source = new CancellationTokenSource();
      Console.WriteLine("new " + _source);

      await ffmpegOutput.CopyToAsync(discordAudioStream, 81920, _source.Token).ContinueWith(task => {
        if (!task.IsCanceled && task.IsFaulted) //supress cancel exception
          Console.WriteLine(task.Exception);
      });
      ffmpegProcess.WaitForExit();
      await discordAudioStream.FlushAsync();

      Console.WriteLine("dispose " + _source);
      _source.Dispose();
      _source = null;
      _settings.currentSong = "";
      _playing = false;
    }

    public async Task StopCurrentStreamAsync()
    {
      //@@@ Currently does not restart playing a song after being ran once
      if (_playing)
      {
        if (_source != null)
        {
          _source.Cancel();
          _source.Dispose();
          _source = null;
        }
        _playing = false;
        _timer.Start();
      }
    }

    private async Task HandleTimerAsync()
    {
      try
      {
        if (_settings.voiceClient != null && _settings.playList.Count > 0 && !_playing)
        {
          _playing = true;
          var path = _settings.playList.Keys.First();

          _settings.playList.TryRemove(_settings.playList.Keys.First(), out VideoInfo video);
          await _message.SendToChannel(331741897737502720, $"Now playing: {video.Title}");
          _settings.currentSong = video.Title;
          await StartStreamAsync(_settings.voiceClient, path);
        }
      }
      catch (Exception e) { Console.WriteLine(e); }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        _timer.Dispose();
        _timer = null;
      }
    }
  }
}

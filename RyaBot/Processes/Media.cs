using Discord.Audio;
using RyaBot.Handlers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RyaBot.Models;

namespace RyaBot.Processes
{
  public class Media : IDisposable
  {
    private readonly Settings _settings;
    private readonly Message _message;

    private CancellationTokenSource _source;
    private System.Timers.Timer _timer;
    private bool _playing;

    public Media(Settings settings, Message message)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(Settings));
      _message = message ?? throw new ArgumentNullException(nameof(Message));

      _timer = new System.Timers.Timer(1000) {AutoReset = false};
      _timer.Elapsed += async (sender, e) => { await HandleTimerAsync(); _timer.Start(); };
      _timer.Start();
    }

    private Process StartFfmpeg(string url)
    {
      var ffmpeg = new ProcessStartInfo
      {
        FileName = "3rd_party\\ffmpeg",
        Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -err_detect ignore_err -i {url} -f s16le -ar 48000 -vn -ac 2 pipe:1 -loglevel fatal",
        UseShellExecute = false,
        RedirectStandardOutput = true,
      };
      return Process.Start(ffmpeg);
    }

    public async Task StartStreamAsync(IAudioClient client, string url)
    {
      var ffmpegProcess = StartFfmpeg(url);
      var ffmpegOutput = ffmpegProcess.StandardOutput.BaseStream;
      var discordAudioStream = client.CreatePCMStream(AudioApplication.Music, bufferMillis: 500);

      _source = new CancellationTokenSource();

      await ffmpegOutput.CopyToAsync(discordAudioStream, 81920, _source.Token).ContinueWith(task =>
      {
        if (!task.IsCanceled && task.IsFaulted) //supress cancel exception
          Console.WriteLine(task.Exception);
      });
      ffmpegProcess.WaitForExit();
      await discordAudioStream.FlushAsync();

      _source.Dispose();
      _source = null;
      _settings.CurrentSong = "";
      _playing = false;
    }

    public void StopCurrentStreamAsync()
    {
      if (!_playing) return;

      if (_source != null)
      {
        _source.Cancel();
        _source.Dispose();
        _source = null;
      }
      _playing = false;
      _timer.Start();
    }

    private async Task HandleTimerAsync()
    {
      if (_settings.VoiceClient != null && _settings.PlayList.Any() && !_playing)
      {
        _playing = true;
        var song = _settings.PlayList.First();
        _settings.PlayList.RemoveAt(0);

        await _message.SendToChannel(331741897737502720, $"Now playing: {song.Title}");
        _settings.CurrentSong = song.Title;
        await StartStreamAsync(_settings.VoiceClient, song.Url);
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing) return;

      _timer.Dispose();
      _timer = null;
    }
  }
}
using Discord.Audio;
using System.Collections.Concurrent;
using YoutubeExplode.Models;

namespace RyaBot.Services
{
  public class Settings
  {
    public string currentSong = "";
    public ConcurrentDictionary<string, VideoInfo> playList = new ConcurrentDictionary<string, VideoInfo>();
    public IAudioClient voiceClient = null;
  }
}

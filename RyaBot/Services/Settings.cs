using Discord.Audio;
using RyaBot.Models;
using System.Collections.Generic;

namespace RyaBot.Services
{
  public class Settings
  {
    public string currentSong = "";
    public HashList<Song> playList = new HashList<Song>();
    public IAudioClient voiceClient = null;
  }
}
